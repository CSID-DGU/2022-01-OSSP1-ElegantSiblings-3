

public enum PROTOCOL : short
{
	BEGIN = 0,

	// �ε�
	START_LOADING = 1,

	// �ε� �Ϸ�
	LOADING_COMPLETED = 2,

	// ���� ����
	GAME_START = 3,

	// ���� ���� (���� ����, �ִ� ũ�� ���)
	MODIFIED_SCORE = 4,

	// ��尡 �̵���
	MOVED_NODE = 5,

	// ���(���� ���)�� ������
	CREATED_NEW_NODE = 6,

	// ���� ����
	GIVE_UP_GAME = 7,

	// ���� ��Ģ�� ���� ���� ���� (�÷��̾� �� 2048�� �޼��� ����� �ִ°�? �׺��� �� ����� �ִ°�?)
	GAME_OVER = 8,

	// ������ ���� ���� üũ
	CHECKING_CONNECTION_STATUS = 9,

	// ���ӷ� ���� ��û
	ENTER_GAME_ROOM_REQ = 10,

	// �г��� ��ȯ
	EXCHANGE_NICKNAME = 11,

	END
}