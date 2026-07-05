/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDprc.h                                                                                                                     */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Prototype declaration for all the Convert related to IOCARD CSCI                                                                */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDprc_h
#define IOCARDprc_h

#include "IOCARDtype.h"
#include "typelib.h"

#ifdef __cplusplus
extern "C" {
#endif

/* HDR_ONLY_MESSAGE_LCU2IO */
void HDR_ONLY_MESSAGE_LCU2IO_CONVERT_TO_IN(PHS_HDRONLYMESSAGELCU2IO* phsPtr, AdiUInt8* intfPtr);
/* HDR_ONLY_MESSAGE_LCU2IO */
AdiInt HDR_ONLY_MESSAGE_LCU2IO_CONVERT_TO_PH(PHS_HDRONLYMESSAGELCU2IO* phsPtr, AdiUInt8* intfPtr);
/* KEEP_ALIVE_MESSAGE */
void KEEP_ALIVE_MESSAGE_CONVERT_TO_IN(PHS_KEEPALIVEMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* KEEP_ALIVE_MESSAGE */
AdiInt KEEP_ALIVE_MESSAGE_CONVERT_TO_PH(PHS_KEEPALIVEMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* SERIAL_LCU2IO_MESSAGE */
void SERIAL_LCU2IO_MESSAGE_CONVERT_TO_IN(PHS_SERIALLCU2IOMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* SERIAL_LCU2IO_MESSAGE */
AdiInt SERIAL_LCU2IO_MESSAGE_CONVERT_TO_PH(PHS_SERIALLCU2IOMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* SET_DISCRETES_MESSAGE */
void SET_DISCRETES_MESSAGE_CONVERT_TO_IN(PHS_SETDISCRETESMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* SET_DISCRETES_MESSAGE */
AdiInt SET_DISCRETES_MESSAGE_CONVERT_TO_PH(PHS_SETDISCRETESMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* SET_SYSTEM_STATE_MESSAGE */
void SET_SYSTEM_STATE_MESSAGE_CONVERT_TO_IN(PHS_SETSYSTEMSTATEMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* SET_SYSTEM_STATE_MESSAGE */
AdiInt SET_SYSTEM_STATE_MESSAGE_CONVERT_TO_PH(PHS_SETSYSTEMSTATEMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* START_INTERFACE_MESSAGE */
void START_INTERFACE_MESSAGE_CONVERT_TO_IN(PHS_STARTINTERFACEMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* START_INTERFACE_MESSAGE */
AdiInt START_INTERFACE_MESSAGE_CONVERT_TO_PH(PHS_STARTINTERFACEMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* STOP_INTERFACE_MESSAGE */
void STOP_INTERFACE_MESSAGE_CONVERT_TO_IN(PHS_STOPINTERFACEMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* STOP_INTERFACE_MESSAGE */
AdiInt STOP_INTERFACE_MESSAGE_CONVERT_TO_PH(PHS_STOPINTERFACEMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* TL_ONLY_MESSAGE_LCU2IO */
void TL_ONLY_MESSAGE_LCU2IO_CONVERT_TO_IN(PHS_TLONLYMESSAGELCU2IO* phsPtr, AdiUInt8* intfPtr);
/* TL_ONLY_MESSAGE_LCU2IO */
AdiInt TL_ONLY_MESSAGE_LCU2IO_CONVERT_TO_PH(PHS_TLONLYMESSAGELCU2IO* phsPtr, AdiUInt8* intfPtr);
/* HDR_ONLY_MESSAGE_IO2LCU */
void HDR_ONLY_MESSAGE_IO2LCU_CONVERT_TO_IN(PHS_HDRONLYMESSAGEIO2LCU* phsPtr, AdiUInt8* intfPtr);
/* HDR_ONLY_MESSAGE_IO2LCU */
AdiInt HDR_ONLY_MESSAGE_IO2LCU_CONVERT_TO_PH(PHS_HDRONLYMESSAGEIO2LCU* phsPtr, AdiUInt8* intfPtr);
/* LOG_DATA_MESSAGE */
void LOG_DATA_MESSAGE_CONVERT_TO_IN(PHS_LOGDATAMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* LOG_DATA_MESSAGE */
AdiInt LOG_DATA_MESSAGE_CONVERT_TO_PH(PHS_LOGDATAMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* NONCE_MESSAGE */
void NONCE_MESSAGE_CONVERT_TO_IN(PHS_NONCEMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* NONCE_MESSAGE */
AdiInt NONCE_MESSAGE_CONVERT_TO_PH(PHS_NONCEMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* REQUEST_STATUS_MESSAGE */
void REQUEST_STATUS_MESSAGE_CONVERT_TO_IN(PHS_REQUESTSTATUSMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* REQUEST_STATUS_MESSAGE */
AdiInt REQUEST_STATUS_MESSAGE_CONVERT_TO_PH(PHS_REQUESTSTATUSMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* SERIAL_IO2LCU_MESSAGE */
void SERIAL_IO2LCU_MESSAGE_CONVERT_TO_IN(PHS_SERIALIO2LCUMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* SERIAL_IO2LCU_MESSAGE */
AdiInt SERIAL_IO2LCU_MESSAGE_CONVERT_TO_PH(PHS_SERIALIO2LCUMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* STATUS_MESSAGE */
void STATUS_MESSAGE_CONVERT_TO_IN(PHS_STATUSMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* STATUS_MESSAGE */
AdiInt STATUS_MESSAGE_CONVERT_TO_PH(PHS_STATUSMESSAGE* phsPtr, AdiUInt8* intfPtr);
/* TL_ONLY_MESSAGE_IO2LCU */
void TL_ONLY_MESSAGE_IO2LCU_CONVERT_TO_IN(PHS_TLONLYMESSAGEIO2LCU* phsPtr, AdiUInt8* intfPtr);
/* TL_ONLY_MESSAGE_IO2LCU */
AdiInt TL_ONLY_MESSAGE_IO2LCU_CONVERT_TO_PH(PHS_TLONLYMESSAGEIO2LCU* phsPtr, AdiUInt8* intfPtr);


/* Convert a message in bus IOCARD_LCU_PR to Interface (using the static physical structure). */
void IOCARD_LCU_PR_ConvertToInterface(AdiInt MsgID, AdiUInt8*  MessageBuffer);

/* Convert a message in bus IOCARD_LCU_PR to Physical (using the static physical structure). */
AdiInt IOCARD_LCU_PR_ConvertToPhysical(AdiInt MsgID,  AdiUInt8*  MessageBuffer);


#ifdef __cplusplus
}
#endif

#endif 
