using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
using System;

public enum PROTOCOL : short
{
	BEGIN = 0,

	CHAT_MSG_REQ = 1,
	CHAT_MSG_ACK = 2,

	END
}