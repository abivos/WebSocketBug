using System;
using System.Collections;
using System.Security.Authentication;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class DebugController : MonoBehaviour
{
	private const int MessagesCount = 30;
	private int _received;

	[SerializeField]
	private Image _image;

	private WebSocket _socket;
	private double _lastPing;
	private double _pingPeriod = 1;

	private int _failedSendCount;
	private const int MAX_FAILED = 3;

	private void Awake()
	{
		_image.color = Color.red;
	}

	public void Click()
	{
		StartCoroutine(SendRoutine());
	}

	private IEnumerator SendRoutine()
	{
		_image.color = Color.yellow;
		_received = 0;

		yield return null;

		_socket = new WebSocket ("wss://echo.websocket.org");

		_socket.OnMessage += (sender, e) =>
		{
			// Debug.Log("echo: " + e.Data);
			Interlocked.Increment(ref _received);
		};

		_socket.Log.Level = LogLevel.Error;
		_socket.Log.Output = (data, path) => Debug.Log(data.ToString());

		if (_socket.IsSecure)
			_socket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;

		_socket.Connect();

		yield return null;

		for (var i = 0; i < MessagesCount; i++)
		{
			var index = i;

			Action<bool> onComplete = complete =>
			{
				// Debug.Log("sent: " + index);
			};

			_socket.SendAsync(index + ": Message testing unity android app hang /n"
									+ "Lorem ipsum dolor sit amet, consectetur adipiscing elit.", onComplete);
		}

		while (_received < MessagesCount)
		{
			if (Time.realtimeSinceStartup - _lastPing > _pingPeriod)
			{
				_lastPing = Time.realtimeSinceStartup;
				PingAsync((pong, crash) =>
				{
					if (crash)
					{
						Debug.Log("[ping] Pong crashed");
						_failedSendCount = MAX_FAILED;
					}
					else
					{
						if (!pong)
						{
							Debug.Log("[ping] Pong failed");
							Interlocked.Increment(ref _failedSendCount);
						}
						else
						{
							Debug.Log("[ping] Pong received");
							_failedSendCount = 0;
						}
					}
				});
			}

			if (_failedSendCount >= MAX_FAILED)
			{
				// Debug.LogError($"FAILURE {_failedSendCount}");
				// yield break;
			}

			yield return null;
		}

		Debug.Log("[DebugController.Click] FINISHED");
		_image.color = Color.green;

		_socket.Close();
		_socket = null;
	}

	private void PingAsync(Action<bool, bool> onComplete)
	{
		Func<bool> ping = _socket.Ping;
		ping.BeginInvoke(ar =>
			{
				try
				{
					var hasPong = ping.EndInvoke(ar);
					if (onComplete != null)
						onComplete(hasPong, false);
				}
				catch (Exception ex)
				{
					Debug.Log("Exception thrown in PingAsync: " + ex);
					if (onComplete != null)
						onComplete(false, true);
				}
			},
			null
		);
	}
}
