using System;
using System.Text;


namespace GameNetwork
{
    /// <summary>
    /// byte[] 버퍼를 참조로 보관하여 데이터 변환을 수행한다.
    /// </summary>
    public class Packet
	{
		public IPeer owner { get; private set; }
		public byte[] buffer { get; private set; }
		public int position { get; private set; }

		public Int16 protocol_id { get; private set; }

		public static Packet Create(Int16 protocol_id)
		{
			Packet packet = PacketBufferManager.Pop();
			packet.SetProtocol(protocol_id);
			return packet;
		}

		public static void Destroy(Packet packet)
		{
			PacketBufferManager.Push(packet);
		}

		public Packet(byte[] buffer, IPeer owner)
		{
			this.buffer = buffer;
			this.position = Defines.HEADERSIZE;
			this.owner = owner;
		}

		public Packet()
		{
			this.buffer = new byte[1024];
		}

		public Int16 PopProtocol_ID()
		{
			return PopInt16();
		}

		public void CopyTo(Packet target)
		{
			target.SetProtocol(this.protocol_id);
			target.Overwrite(this.buffer, this.position);
		}

		public void Overwrite(byte[] source, int position)
		{
			Array.Copy(source, this.buffer, source.Length);
			this.position = position;
		}

		public byte PopByte()
		{
			byte data = (byte)BitConverter.ToInt16(this.buffer, this.position);
			this.position += sizeof(byte);
			return data;
		}

		public Int16 PopInt16()
		{
			Int16 data = BitConverter.ToInt16(this.buffer, this.position);
			this.position += sizeof(Int16);
			return data;
		}

		public Int32 PopInt32()
		{
			Int32 data = BitConverter.ToInt32(this.buffer, this.position);
			this.position += sizeof(Int32);
			return data;
		}

		public string PopString()
		{
			// 문자열 길이는 0 ~ 32767
			Int16 len = BitConverter.ToInt16(this.buffer, this.position);
			this.position += sizeof(Int16);

			// utf8로 인코딩
			string data = System.Text.Encoding.UTF8.GetString(this.buffer, this.position, len);
			this.position += len;

			return data;
		}



		public void SetProtocol(Int16 protocol_id)
		{
			this.protocol_id = protocol_id;
			this.position = Defines.HEADERSIZE;
			PushInt16(protocol_id);
		}

		public void RecordSize()
		{
			Int16 body_size = (Int16)(this.position - Defines.HEADERSIZE);
			byte[] header = BitConverter.GetBytes(body_size);
			header.CopyTo(this.buffer, 0);
		}

		public void PushInt16(Int16 data)
		{
			byte[] temp_buffer = BitConverter.GetBytes(data);
			temp_buffer.CopyTo(this.buffer, this.position);
			this.position += temp_buffer.Length;
		}

		public void Push(byte data)
		{
			byte[] temp_buffer = BitConverter.GetBytes(data);
			temp_buffer.CopyTo(this.buffer, this.position);
			this.position += sizeof(byte);
		}

		public void Push(Int16 data)
		{
			byte[] temp_buffer = BitConverter.GetBytes(data);
			temp_buffer.CopyTo(this.buffer, this.position);
			this.position += temp_buffer.Length;
		}

		public void Push(Int32 data)
		{
			byte[] temp_buffer = BitConverter.GetBytes(data);
			temp_buffer.CopyTo(this.buffer, this.position);
			this.position += temp_buffer.Length;
		}

		public void Push(string data)
		{
			byte[] tempBuffer = Encoding.UTF8.GetBytes(data);

			Int16 len = (Int16)tempBuffer.Length;
			byte[] lenBuffer = BitConverter.GetBytes(len);
			lenBuffer.CopyTo(this.buffer, this.position);
			this.position += sizeof(Int16);

			tempBuffer.CopyTo(this.buffer, this.position);
			this.position += tempBuffer.Length;
		}
	}
}
