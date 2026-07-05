/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDpri.h                                                                                                                     */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Prototype declaration for all INIT Functions related to IOCARD CSCI                                                             */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDpri_h
#define IOCARDpri_h

#include "IOCARDtype.h"
#include "typelib.h"

#ifdef __cplusplus
extern "C" {
#endif

/* HDR_ONLY_MESSAGE_IO2LCU */
void HdrOnlyMessageIo2lcu_INIT(PHS_HDRONLYMESSAGEIO2LCU* phsPtr);
/* HDR_ONLY_MESSAGE_LCU2IO */
void HdrOnlyMessageLcu2io_INIT(PHS_HDRONLYMESSAGELCU2IO* phsPtr);
/* KEEP_ALIVE_MESSAGE */
void KeepAliveMessage_INIT(PHS_KEEPALIVEMESSAGE* phsPtr);
/* LOG_DATA_MESSAGE */
void LogDataMessage_INIT(PHS_LOGDATAMESSAGE* phsPtr);
/* NONCE_MESSAGE */
void NonceMessage_INIT(PHS_NONCEMESSAGE* phsPtr);
/* REQUEST_STATUS_MESSAGE */
void RequestStatusMessage_INIT(PHS_REQUESTSTATUSMESSAGE* phsPtr);
/* SERIAL_IO2LCU_MESSAGE */
void SerialIo2lcuMessage_INIT(PHS_SERIALIO2LCUMESSAGE* phsPtr);
/* SERIAL_LCU2IO_MESSAGE */
void SerialLcu2ioMessage_INIT(PHS_SERIALLCU2IOMESSAGE* phsPtr);
/* SET_DISCRETES_MESSAGE */
void SetDiscretesMessage_INIT(PHS_SETDISCRETESMESSAGE* phsPtr);
/* SET_SYSTEM_STATE_MESSAGE */
void SetSystemStateMessage_INIT(PHS_SETSYSTEMSTATEMESSAGE* phsPtr);
/* START_INTERFACE_MESSAGE */
void StartInterfaceMessage_INIT(PHS_STARTINTERFACEMESSAGE* phsPtr);
/* STATUS_MESSAGE */
void StatusMessage_INIT(PHS_STATUSMESSAGE* phsPtr);
/* STOP_INTERFACE_MESSAGE */
void StopInterfaceMessage_INIT(PHS_STOPINTERFACEMESSAGE* phsPtr);
/* TL_ONLY_MESSAGE_IO2LCU */
void TlOnlyMessageIo2lcu_INIT(PHS_TLONLYMESSAGEIO2LCU* phsPtr);
/* TL_ONLY_MESSAGE_LCU2IO */
void TlOnlyMessageLcu2io_INIT(PHS_TLONLYMESSAGELCU2IO* phsPtr);

/* Init message in bus IOCARD_LCU_PR(using the static physical structure). */
void IOCARD_LCU_PR_INIT_Message(AdiInt MsgID);


#ifdef __cplusplus
}
#endif

#endif 
