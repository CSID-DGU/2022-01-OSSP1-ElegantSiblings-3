using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;

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

	// Ŭ���̾�Ʈ�� �� ������ �������� �˸���
	TURN_FINISHED_REQ = 7,

	// ���� �÷��̾ ���� ���� �����Ǿ���
	ROOM_REMOVED = 8,

	// ���ӹ� ���� ��û
	ENTER_GAME_ROOM_REQ = 9,

	// ���� ����
	GAME_OVER = 10,

	END
}