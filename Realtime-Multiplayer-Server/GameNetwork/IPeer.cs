

namespace GameNetwork
{
    /// <summary>
    /// 서버와 클라이언트에서 공통으로 사용하는 세션 객체
    /// 서버(= 클라이언트 객체), 클라이언트(= 접속한 서버 객체)
    /// </summary>
    public interface IPeer
	{
		/// <summary>
		/// 이 매소드가 완료된 이후 다음 패킷이 들어오기 때문에 클라이언트가 보낸 패킷 순서는 보장이 된다.
		/// </summary>
		void OnMessage(Const<byte[]> buffer);

		/// <summary>
		/// 원격 연결이 끊겼을 때 호출 된다.
		/// 이 매소드가 호출된 이후부터는 데이터 전송이 불가능하다.
		/// </summary>
		void OnRemoved();

		void Send(Packet msg);

		void Disconnect();

		void ProcessUserOperation(Packet msg);
	}
}
