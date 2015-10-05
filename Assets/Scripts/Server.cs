using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using UnityEngine.Events;

public class Server : MonoBehaviour 
{
	public int port;
	public MessageEvent onMessage;
	public enum IPChoices {
		Loopback,
		Any,
		Specific
	}
	public IPChoices ip;
	public string specificIpAddr;
	
	protected bool messageReady;
	protected string message;
	
	private TcpListener tcp_Listener = null;
	private Thread mThread;
	private bool mRunning;
	
	void Awake() {
		mRunning = true;
		ThreadStart ts = new ThreadStart(Receive);
		mThread = new Thread(ts);
		mThread.Start();
		print("Thread done...");
	}
	
	public void stopListening() {
		mRunning = false;
	}
	
	void Receive() {
		IPAddress addr = IPAddress.Loopback;
		switch(ip){
			case IPChoices.Any:
				addr = IPAddress.Any;
			break;
			case IPChoices.Specific:
				addr = IPAddress.Parse(specificIpAddr);				
			break;
		}
		
		tcp_Listener = new TcpListener(addr, port);
		tcp_Listener.Start();
		print("Server Start");
		while (mRunning)
		{
			// check if new connections are pending, if not, be nice and sleep 100ms
			if (!tcp_Listener.Pending()){
				Thread.Sleep(100);
			}
			else {
				Socket ss = tcp_Listener.AcceptSocket();
				byte[] tempbuffer = new byte[10000];
				int length = ss.Receive(tempbuffer); // received byte array from client
				message = System.Text.Encoding.Default.GetString(tempbuffer,0,length);
				//message = GetString(tempbuffer,length);
				messageReady = true;
				ss.Send( GetBytes("OK\n\n") );
				ss.Close();
			}
		}
	}
	
	void Update() {
		if( messageReady ){
			messageReady = false;
			onMessage.Invoke(message);
		}
	}
	
	void OnApplicationQuit() { // stop listening thread
		stopListening();// wait for listening thread to terminate (max. 500ms)
		if( mThread != null ) mThread.Join(500);
	}
	
	static byte[] GetBytes(string str)
	{
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}
	
	static string GetString(byte[] bytes, int length)
	{
		char[] chars = new char[length / sizeof(char)];
		System.Buffer.BlockCopy(bytes, 0, chars, 0, length);
		return new string(chars);
	}
}

[System.Serializable]
public class MessageEvent : UnityEvent<string> 
{

}