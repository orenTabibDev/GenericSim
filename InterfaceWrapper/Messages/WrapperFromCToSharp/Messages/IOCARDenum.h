/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDenum.h                                                                                                                    */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Enumerated Types declaration                                                                                                    */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDenum_h
#define IOCARDenum_h


#ifdef __cplusplus
extern "C" {
#endif

/* E_BOOLEAN */
#ifndef _ENUM_TYPE_E_BOOLEAN
#define _ENUM_TYPE_E_BOOLEAN
typedef enum {
	E_BOOLEAN_FALSE                                = 0,
	E_BOOLEAN_TRUE                                 = 1
}E_BOOLEAN;
#endif /* _ENUM_TYPE_E_BOOLEAN*/

/* E_CBIT_STATUS */
#ifndef _ENUM_TYPE_E_CBIT_STATUS
#define _ENUM_TYPE_E_CBIT_STATUS
typedef enum {
	E_CBIT_STATUS_CBIT_PASS                        = 0,
	E_CBIT_STATUS_CBIT_FAIL                        = 1,
	E_CBIT_STATUS_CBIT_FATAL                       = 2
}E_CBIT_STATUS;
#endif /* _ENUM_TYPE_E_CBIT_STATUS*/

/* E_DISCRETE_ID */
#ifndef _ENUM_TYPE_E_DISCRETE_ID
#define _ENUM_TYPE_E_DISCRETE_ID
typedef enum {
	E_DISCRETE_ID_MU_RTF_CMD                       = 0,
	E_DISCRETE_ID_MU2FIB_SW_ILLUM_CMD              = 1,
	E_DISCRETE_ID_MU2IO_MSU1_POWER_ON              = 2,
	E_DISCRETE_ID_MU2IO_MSU2_POWER_ON              = 3,
	E_DISCRETE_ID_MU2IO_MSU3_POWER_ON              = 4,
	E_DISCRETE_ID_MU2IO_MSU4_POWER_ON              = 5,
	E_DISCRETE_ID_SSTU_PWR_ON_CMD                  = 6,
	E_DISCRETE_ID_LPS_PWR_ON_CMD                   = 7,
	E_DISCRETE_ID_STRU_PWR_ON_CMD                  = 8,
	E_DISCRETE_ID_MU2MSU1_MSL1_F_EN                = 9,
	E_DISCRETE_ID_MU2MSU1_MSL2_F_EN                = 10,
	E_DISCRETE_ID_MU2MSU1_MSL3_F_EN                = 11,
	E_DISCRETE_ID_MU2MSU1_MSL4_F_EN                = 12,
	E_DISCRETE_ID_MU2MSU1_MSL1_PTF                 = 13,
	E_DISCRETE_ID_MU2MSU1_MSL2_PTF                 = 14,
	E_DISCRETE_ID_MU2MSU1_MSL3_PTF                 = 15,
	E_DISCRETE_ID_MU2MSU1_MSL4_PTF                 = 16,
	E_DISCRETE_ID_MU2MSU1_F_CMD                    = 17,
	E_DISCRETE_ID_MU2MSU1_BTLS_CMD                 = 18,
	E_DISCRETE_ID_MU2MSU2_MSL1_F_EN                = 19,
	E_DISCRETE_ID_MU2MSU2_MSL2_F_EN                = 20,
	E_DISCRETE_ID_MU2MSU2_MSL3_F_EN                = 21,
	E_DISCRETE_ID_MU2MSU2_MSL4_F_EN                = 22,
	E_DISCRETE_ID_MU2MSU2_MSL1_PTF                 = 23,
	E_DISCRETE_ID_MU2MSU2_MSL2_PTF                 = 24,
	E_DISCRETE_ID_MU2MSU2_MSL3_PTF                 = 25,
	E_DISCRETE_ID_MU2MSU2_MSL4_PTF                 = 26,
	E_DISCRETE_ID_MU2MSU2_F_CMD                    = 27,
	E_DISCRETE_ID_MU2MSU2_BTLS_CMD                 = 28,
	E_DISCRETE_ID_MU2MSU3_MSL1_F_EN                = 29,
	E_DISCRETE_ID_MU2MSU3_MSL2_F_EN                = 30,
	E_DISCRETE_ID_MU2MSU3_MSL3_F_EN                = 31,
	E_DISCRETE_ID_MU2MSU3_MSL4_F_EN                = 32,
	E_DISCRETE_ID_MU2MSU3_MSL1_PTF                 = 33,
	E_DISCRETE_ID_MU2MSU3_MSL2_PTF                 = 34,
	E_DISCRETE_ID_MU2MSU3_MSL3_PTF                 = 35,
	E_DISCRETE_ID_MU2MSU3_MSL4_PTF                 = 36,
	E_DISCRETE_ID_MU2MSU3_F_CMD                    = 37,
	E_DISCRETE_ID_MU2MSU3_BTLS_CMD                 = 38,
	E_DISCRETE_ID_MU2MSU4_MSL1_F_EN                = 39,
	E_DISCRETE_ID_MU2MSU4_MSL2_F_EN                = 40,
	E_DISCRETE_ID_MU2MSU4_MSL3_F_EN                = 41,
	E_DISCRETE_ID_MU2MSU4_MSL4_F_EN                = 42,
	E_DISCRETE_ID_MU2MSU4_MSL1_PTF                 = 43,
	E_DISCRETE_ID_MU2MSU4_MSL2_PTF                 = 44,
	E_DISCRETE_ID_MU2MSU4_MSL3_PTF                 = 45,
	E_DISCRETE_ID_MU2MSU4_MSL4_PTF                 = 46,
	E_DISCRETE_ID_MU2MSU4_F_CMD                    = 47,
	E_DISCRETE_ID_MU2MSU4_BTLS_CMD                 = 48,
	E_DISCRETE_ID_CAST_REF                         = 49,
	E_DISCRETE_ID_TEST_REF                         = 50,
	E_DISCRETE_ID_MISN_REF                         = 51,
	E_DISCRETE_ID_SAFE_REF                         = 52,
	E_DISCRETE_ID_FIB_FKEY_ARM                     = 53,
	E_DISCRETE_ID_FIB_F_PB                         = 54,
	E_DISCRETE_ID_SSTU_INHIBIT_CTRL                = 55,
	E_DISCRETE_ID_SSTU_VOK_IND                     = 56,
	E_DISCRETE_ID_LPS_PWR_INHIBIT                  = 57,
	E_DISCRETE_ID_LPS_VOK_IND                      = 58,
	E_DISCRETE_ID_STRU_VOK_IND                     = 59,
	E_DISCRETE_ID_STRU_PWR_INHIBIT                 = 60,
	E_DISCRETE_ID_LPS_CB_IND                       = 61,
	E_DISCRETE_ID_STRU_TX_ON_IND                   = 62,
	E_DISCRETE_ID_MU_ON_IND                        = 63,
	E_DISCRETE_ID_MSU1_CAST_MODE                   = 64,
	E_DISCRETE_ID_MSU1_MISN_MODE                   = 65,
	E_DISCRETE_ID_MSU1_TST_MODE                    = 66,
	E_DISCRETE_ID_MSU1_CB_IND                      = 67,
	E_DISCRETE_ID_MSU1_BTLS_CMD                    = 68,
	E_DISCRETE_ID_MSU1_OVER_TEMP_IND               = 69,
	E_DISCRETE_ID_MSU1_OK_IND                      = 70,
	E_DISCRETE_ID_MSU1_ON_IND                      = 71,
	E_DISCRETE_ID_MSU1_VOK_IND                     = 72,
	E_DISCRETE_ID_MSU1_POWER_ON_OR_RET             = 73,
	E_DISCRETE_ID_MSU1_HOLD_ON                     = 74,
	E_DISCRETE_ID_MSU1_MSL1_F_EN                   = 75,
	E_DISCRETE_ID_MSU1_MSL2_F_EN                   = 76,
	E_DISCRETE_ID_MSU1_MSL3_F_EN                   = 77,
	E_DISCRETE_ID_MSU1_MSL4_F_EN                   = 78,
	E_DISCRETE_ID_MSU1_MSL1_PTF                    = 79,
	E_DISCRETE_ID_MSU1_MSL2_PTF                    = 80,
	E_DISCRETE_ID_MSU1_MSL3_PTF                    = 81,
	E_DISCRETE_ID_MSU1_MSL4_PTF                    = 82,
	E_DISCRETE_ID_MSU1_MLS1_CMD_RET                = 83,
	E_DISCRETE_ID_MSU1_MLS2_CMD_RET                = 84,
	E_DISCRETE_ID_MSU1_MLS3_CMD_RET                = 85,
	E_DISCRETE_ID_MSU1_MLS4_CMD_RET                = 86,
	E_DISCRETE_ID_MSU1_F_CMD                       = 87,
	E_DISCRETE_ID_MSU1_DIS_F                       = 88,
	E_DISCRETE_ID_MSU1_2_SEC_DELAY                 = 89,
	E_DISCRETE_ID_MSU2_CAST_MODE                   = 90,
	E_DISCRETE_ID_MSU2_MISN_MODE                   = 91,
	E_DISCRETE_ID_MSU2_TST_MODE                    = 92,
	E_DISCRETE_ID_MSU2_CB_IND                      = 93,
	E_DISCRETE_ID_MSU2_BTLS_CMD                    = 94,
	E_DISCRETE_ID_MSU2_OVER_TEMP_IND               = 95,
	E_DISCRETE_ID_MSU2_OK_IND                      = 96,
	E_DISCRETE_ID_MSU2_ON_IND                      = 97,
	E_DISCRETE_ID_MSU2_VOK_IND                     = 98,
	E_DISCRETE_ID_MSU2_POWER_ON_OR_RET             = 99,
	E_DISCRETE_ID_MSU2_HOLD_ON                     = 100,
	E_DISCRETE_ID_MSU2_MSL1_F_EN                   = 101,
	E_DISCRETE_ID_MSU2_MSL2_F_EN                   = 102,
	E_DISCRETE_ID_MSU2_MSL3_F_EN                   = 103,
	E_DISCRETE_ID_MSU2_MSL4_F_EN                   = 104,
	E_DISCRETE_ID_MSU2_MSL1_PTF                    = 105,
	E_DISCRETE_ID_MSU2_MSL2_PTF                    = 106,
	E_DISCRETE_ID_MSU2_MSL3_PTF                    = 107,
	E_DISCRETE_ID_MSU2_MSL4_PTF                    = 108,
	E_DISCRETE_ID_MSU2_MLS1_CMD_RET                = 109,
	E_DISCRETE_ID_MSU2_MLS2_CMD_RET                = 110,
	E_DISCRETE_ID_MSU2_MLS3_CMD_RET                = 111,
	E_DISCRETE_ID_MSU2_MLS4_CMD_RET                = 112,
	E_DISCRETE_ID_MSU2_F_CMD                       = 113,
	E_DISCRETE_ID_MSU2_DIS_F                       = 114,
	E_DISCRETE_ID_MSU2_2_SEC_DELAY                 = 115,
	E_DISCRETE_ID_MSU3_CAST_MODE                   = 116,
	E_DISCRETE_ID_MSU3_MISN_MODE                   = 117,
	E_DISCRETE_ID_MSU3_TST_MODE                    = 118,
	E_DISCRETE_ID_MSU3_CB_IND                      = 119,
	E_DISCRETE_ID_MSU3_BTLS_CMD                    = 120,
	E_DISCRETE_ID_MSU3_OVER_TEMP_IND               = 121,
	E_DISCRETE_ID_MSU3_OK_IND                      = 122,
	E_DISCRETE_ID_MSU3_ON_IND                      = 123,
	E_DISCRETE_ID_MSU3_VOK_IND                     = 124,
	E_DISCRETE_ID_MSU3_POWER_ON_OR_RET             = 125,
	E_DISCRETE_ID_MSU3_HOLD_ON                     = 126,
	E_DISCRETE_ID_MSU3_MSL1_F_EN                   = 127,
	E_DISCRETE_ID_MSU3_MSL2_F_EN                   = 128,
	E_DISCRETE_ID_MSU3_MSL3_F_EN                   = 129,
	E_DISCRETE_ID_MSU3_MSL4_F_EN                   = 130,
	E_DISCRETE_ID_MSU3_MSL1_PTF                    = 131,
	E_DISCRETE_ID_MSU3_MSL2_PTF                    = 132,
	E_DISCRETE_ID_MSU3_MSL3_PTF                    = 133,
	E_DISCRETE_ID_MSU3_MSL4_PTF                    = 134,
	E_DISCRETE_ID_MSU3_MLS1_CMD_RET                = 135,
	E_DISCRETE_ID_MSU3_MLS2_CMD_RET                = 136,
	E_DISCRETE_ID_MSU3_MLS3_CMD_RET                = 137,
	E_DISCRETE_ID_MSU3_MLS4_CMD_RET                = 138,
	E_DISCRETE_ID_MSU3_F_CMD                       = 139,
	E_DISCRETE_ID_MSU3_DIS_F                       = 140,
	E_DISCRETE_ID_MSU3_2_SEC_DELAY                 = 141,
	E_DISCRETE_ID_MSU4_CAST_MODE                   = 142,
	E_DISCRETE_ID_MSU4_MISN_MODE                   = 143,
	E_DISCRETE_ID_MSU4_TST_MODE                    = 144,
	E_DISCRETE_ID_MSU4_CB_IND                      = 145,
	E_DISCRETE_ID_MSU4_BTLS_CMD                    = 146,
	E_DISCRETE_ID_MSU4_OVER_TEMP_IND               = 147,
	E_DISCRETE_ID_MSU4_OK_IND                      = 148,
	E_DISCRETE_ID_MSU4_ON_IND                      = 149,
	E_DISCRETE_ID_MSU4_VOK_IND                     = 150,
	E_DISCRETE_ID_MSU4_POWER_ON_OR_RET             = 151,
	E_DISCRETE_ID_MSU4_HOLD_ON                     = 152,
	E_DISCRETE_ID_MSU4_MSL1_F_EN                   = 153,
	E_DISCRETE_ID_MSU4_MSL2_F_EN                   = 154,
	E_DISCRETE_ID_MSU4_MSL3_F_EN                   = 155,
	E_DISCRETE_ID_MSU4_MSL4_F_EN                   = 156,
	E_DISCRETE_ID_MSU4_MSL1_PTF                    = 157,
	E_DISCRETE_ID_MSU4_MSL2_PTF                    = 158,
	E_DISCRETE_ID_MSU4_MSL3_PTF                    = 159,
	E_DISCRETE_ID_MSU4_MSL4_PTF                    = 160,
	E_DISCRETE_ID_MSU4_MLS1_CMD_RET                = 161,
	E_DISCRETE_ID_MSU4_MLS2_CMD_RET                = 162,
	E_DISCRETE_ID_MSU4_MLS3_CMD_RET                = 163,
	E_DISCRETE_ID_MSU4_MLS4_CMD_RET                = 164,
	E_DISCRETE_ID_MSU4_F_CMD                       = 165,
	E_DISCRETE_ID_MSU4_DIS_F                       = 166,
	E_DISCRETE_ID_MSU4_2_SEC_DELAY                 = 167,
	E_DISCRETE_ID_FIB_CAST_MODE                    = 168,
	E_DISCRETE_ID_FIB_TEST_MODE                    = 169,
	E_DISCRETE_ID_FIB_MISN_MODE                    = 170,
	E_DISCRETE_ID_FIB_SAFE_MODE                    = 171,
	E_DISCRETE_ID_FIB_PTF                          = 172,
	E_DISCRETE_ID_FIB_F_EN                         = 173,
	E_DISCRETE_ID_FIB2LU_PERMIT1                   = 174,
	E_DISCRETE_ID_FIB2LU_PERMIT2                   = 175,
	E_DISCRETE_ID_FIB2LU_PERMIT3                   = 176,
	E_DISCRETE_ID_FIB2LU_PERMIT4                   = 177,
	E_DISCRETE_ID_LU2FIB_BLOCK1                    = 178,
	E_DISCRETE_ID_LU2FIB_BLOCK2                    = 179,
	E_DISCRETE_ID_LU2FIB_BLOCK3                    = 180,
	E_DISCRETE_ID_LU2FIB_BLOCK4                    = 181
}E_DISCRETE_ID;
#endif /* _ENUM_TYPE_E_DISCRETE_ID*/

/* E_ERROR_ID */
#ifndef _ENUM_TYPE_E_ERROR_ID
#define _ENUM_TYPE_E_ERROR_ID
typedef enum {
	E_ERROR_ID_SUCCESS                             = 0,
	E_ERROR_ID_INCORRECT_ID                        = 1,
	E_ERROR_ID_INCORRECT_LENGTH                    = 2,
	E_ERROR_ID_INCORRECT_CRC                       = 3,
	E_ERROR_ID_EXECUTION_ERROR                     = 4,
	E_ERROR_ID_REPEATED_SEQ_COUNT                  = 5,
	E_ERROR_ID_INCORRECT_NONCE_VAL                 = 6
}E_ERROR_ID;
#endif /* _ENUM_TYPE_E_ERROR_ID*/

/* E_LOG_ID */
#ifndef _ENUM_TYPE_E_LOG_ID
#define _ENUM_TYPE_E_LOG_ID
typedef enum {
	E_LOG_ID_UNDEFINED_1                           = 1,
	E_LOG_ID_UNDEFINED_2                           = 2,
	E_LOG_ID_UNDEFINED_3                           = 3
}E_LOG_ID;
#endif /* _ENUM_TYPE_E_LOG_ID*/

/* E_LOG_LEVEL */
#ifndef _ENUM_TYPE_E_LOG_LEVEL
#define _ENUM_TYPE_E_LOG_LEVEL
typedef enum {
	E_LOG_LEVEL_NOT_ASSIGNED                       = 0,
	E_LOG_LEVEL_VERBOSE                            = 1,
	E_LOG_LEVEL_DEBUG                              = 2,
	E_LOG_LEVEL_WARN                               = 3,
	E_LOG_LEVEL_ERROR                              = 4,
	E_LOG_LEVEL_CRIT_ERR                           = 5
}E_LOG_LEVEL;
#endif /* _ENUM_TYPE_E_LOG_LEVEL*/

/* E_MESSAGE_TYPE */
#ifndef _ENUM_TYPE_E_MESSAGE_TYPE
#define _ENUM_TYPE_E_MESSAGE_TYPE
typedef enum {
	E_MESSAGE_TYPE_NOT_ASSIGNED                    = 0,
	E_MESSAGE_TYPE_SET_DISCRETES                   = 1,
	E_MESSAGE_TYPE_SERIAL_LCU2IO                   = 2,
	E_MESSAGE_TYPE_SET_SYSTEM_STATE                = 3,
	E_MESSAGE_TYPE_PROGRAM_DATA                    = 4,
	E_MESSAGE_TYPE_START_UART_INTERFACE            = 5,
	E_MESSAGE_TYPE_STOP_UART_INTERFACE             = 6,
	E_MESSAGE_TYPE_KEEP_ALIVE                      = 7,
	E_MESSAGE_TYPE_REQUEST_STATUS                  = 101,
	E_MESSAGE_TYPE_MONITORED_DATA                  = 102,
	E_MESSAGE_TYPE_SERIAL_IO2LCU                   = 103,
	E_MESSAGE_TYPE_LOG_OR_ERROR                    = 104,
	E_MESSAGE_TYPE_NONCE                           = 200
}E_MESSAGE_TYPE;
#endif /* _ENUM_TYPE_E_MESSAGE_TYPE*/

/* E_PARITY */
#ifndef _ENUM_TYPE_E_PARITY
#define _ENUM_TYPE_E_PARITY
typedef enum {
	E_PARITY_OFF                                   = 0,
	E_PARITY_ODD                                   = 1,
	E_PARITY_EVEN                                  = 2
}E_PARITY;
#endif /* _ENUM_TYPE_E_PARITY*/

/* E_REGISTRATION_STATUS */
#ifndef _ENUM_TYPE_E_REGISTRATION_STATUS
#define _ENUM_TYPE_E_REGISTRATION_STATUS
typedef enum {
	E_REGISTRATION_STATUS_PBIT_PASS                = 0,
	E_REGISTRATION_STATUS_PBIT_FAIL                = 1,
	E_REGISTRATION_STATUS_PBIT_FATAL               = 2,
	E_REGISTRATION_STATUS_NOT_REGISTERED           = 3
}E_REGISTRATION_STATUS;
#endif /* _ENUM_TYPE_E_REGISTRATION_STATUS*/

/* E_UART_ID */
#ifndef _ENUM_TYPE_E_UART_ID
#define _ENUM_TYPE_E_UART_ID
typedef enum {
	E_UART_ID_NOT_ASSIGNED                         = 0,
	E_UART_ID_NAV_10HZ                             = 1,
	E_UART_ID_NAV_100HZ                            = 2,
	E_UART_ID_NAV_NMEA                             = 3,
	E_UART_ID_ANCU_1                               = 4,
	E_UART_ID_ANCU_2                               = 5
}E_UART_ID;
#endif /* _ENUM_TYPE_E_UART_ID*/

/* E_UNIT_ID */
#ifndef _ENUM_TYPE_E_UNIT_ID
#define _ENUM_TYPE_E_UNIT_ID
typedef enum {
	E_UNIT_ID_NOT_ASSIGNED                         = 0,
	E_UNIT_ID_IOUNIT                               = 1,
	E_UNIT_ID_FIB                                  = 2,
	E_UNIT_ID_LUE2D_1                              = 3,
	E_UNIT_ID_LUE2D_2                              = 4,
	E_UNIT_ID_LUE2D_3                              = 5,
	E_UNIT_ID_LUE2D_4                              = 6
}E_UNIT_ID;
#endif /* _ENUM_TYPE_E_UNIT_ID*/

/* E_UNIT_STATE */
#ifndef _ENUM_TYPE_E_UNIT_STATE
#define _ENUM_TYPE_E_UNIT_STATE
typedef enum {
	E_UNIT_STATE_IDLE                              = 0,
	E_UNIT_STATE_OPERATIONAL                       = 1,
	E_UNIT_STATE_PROGRAMMING                       = 2
}E_UNIT_STATE;
#endif /* _ENUM_TYPE_E_UNIT_STATE*/


/* IOCARD_LCU_PR */
#ifndef _MESSAGES_ENUM_TYPE_IOCARD_LCU_PR
#define _MESSAGES_ENUM_TYPE_IOCARD_LCU_PR
typedef enum {
	IOCARD_LCU_PR_MsgEnum_no_code                   = 0,
	IOCARD_LCU_PR_MsgEnum_HDR_ONLY_MESSAGE_LCU2IO  = 1002,
	IOCARD_LCU_PR_MsgEnum_KEEP_ALIVE_MESSAGE       = 7,
	IOCARD_LCU_PR_MsgEnum_SERIAL_LCU2IO_MESSAGE    = 2,
	IOCARD_LCU_PR_MsgEnum_SET_DISCRETES_MESSAGE    = 1,
	IOCARD_LCU_PR_MsgEnum_SET_SYSTEM_STATE_MESSAGE = 3,
	IOCARD_LCU_PR_MsgEnum_START_INTERFACE_MESSAGE  = 5,
	IOCARD_LCU_PR_MsgEnum_STOP_INTERFACE_MESSAGE   = 6,
	IOCARD_LCU_PR_MsgEnum_TL_ONLY_MESSAGE_LCU2IO   = 1004,
	IOCARD_LCU_PR_MsgEnum_HDR_ONLY_MESSAGE_IO2LCU  = 1001,
	IOCARD_LCU_PR_MsgEnum_LOG_DATA_MESSAGE         = 104,
	IOCARD_LCU_PR_MsgEnum_NONCE_MESSAGE            = 200,
	IOCARD_LCU_PR_MsgEnum_REQUEST_STATUS_MESSAGE   = 101,
	IOCARD_LCU_PR_MsgEnum_SERIAL_IO2LCU_MESSAGE    = 103,
	IOCARD_LCU_PR_MsgEnum_STATUS_MESSAGE           = 102,
	IOCARD_LCU_PR_MsgEnum_TL_ONLY_MESSAGE_IO2LCU   = 1003
}IOCARD_LCU_PR_OPCODE;
#endif /* _MESSAGES_ENUM_TYPE_IOCARD_LCU_PR*/



#ifdef __cplusplus
}
#endif

#endif 
