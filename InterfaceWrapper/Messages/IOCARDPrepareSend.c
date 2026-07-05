/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDPrepareSend.c                                                                                                             */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Prepare messages for transmission (init special elements from flag structure)                                                   */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDPrepareSend_c
#define IOCARDPrepareSend_c

#include "IOCARDPrepareSend.h"

#ifdef __cplusplus
extern "C" {
#endif

/*HDR_ONLY_MESSAGE_IO2LCU*/
void HDR_ONLY_MESSAGE_IO2LCU_PrepareSend(PHS_HDRONLYMESSAGEIO2LCU* phsPtr, PHS_HDRONLYMESSAGEIO2LCU_FLAG* phsFlgPtr)
{
}

/*HDR_ONLY_MESSAGE_LCU2IO*/
void HDR_ONLY_MESSAGE_LCU2IO_PrepareSend(PHS_HDRONLYMESSAGELCU2IO* phsPtr, PHS_HDRONLYMESSAGELCU2IO_FLAG* phsFlgPtr)
{
}

/*KEEP_ALIVE_MESSAGE*/
void KEEP_ALIVE_MESSAGE_PrepareSend(PHS_KEEPALIVEMESSAGE* phsPtr, PHS_KEEPALIVEMESSAGE_FLAG* phsFlgPtr)
{
}

/*LOG_DATA_MESSAGE*/
void LOG_DATA_MESSAGE_PrepareSend(PHS_LOGDATAMESSAGE* phsPtr, PHS_LOGDATAMESSAGE_FLAG* phsFlgPtr)
{
}

/*NONCE_MESSAGE*/
void NONCE_MESSAGE_PrepareSend(PHS_NONCEMESSAGE* phsPtr, PHS_NONCEMESSAGE_FLAG* phsFlgPtr)
{
}

/*REQUEST_STATUS_MESSAGE*/
void REQUEST_STATUS_MESSAGE_PrepareSend(PHS_REQUESTSTATUSMESSAGE* phsPtr, PHS_REQUESTSTATUSMESSAGE_FLAG* phsFlgPtr)
{
}

/*SERIAL_IO2LCU_MESSAGE*/
void SERIAL_IO2LCU_MESSAGE_PrepareSend(PHS_SERIALIO2LCUMESSAGE* phsPtr, PHS_SERIALIO2LCUMESSAGE_FLAG* phsFlgPtr)
{
}

/*SERIAL_LCU2IO_MESSAGE*/
void SERIAL_LCU2IO_MESSAGE_PrepareSend(PHS_SERIALLCU2IOMESSAGE* phsPtr, PHS_SERIALLCU2IOMESSAGE_FLAG* phsFlgPtr)
{
}

/*SET_DISCRETES_MESSAGE*/
void SET_DISCRETES_MESSAGE_PrepareSend(PHS_SETDISCRETESMESSAGE* phsPtr, PHS_SETDISCRETESMESSAGE_FLAG* phsFlgPtr)
{
}

/*SET_SYSTEM_STATE_MESSAGE*/
void SET_SYSTEM_STATE_MESSAGE_PrepareSend(PHS_SETSYSTEMSTATEMESSAGE* phsPtr, PHS_SETSYSTEMSTATEMESSAGE_FLAG* phsFlgPtr)
{
}

/*START_INTERFACE_MESSAGE*/
void START_INTERFACE_MESSAGE_PrepareSend(PHS_STARTINTERFACEMESSAGE* phsPtr, PHS_STARTINTERFACEMESSAGE_FLAG* phsFlgPtr)
{
}

/*STATUS_MESSAGE*/
void STATUS_MESSAGE_PrepareSend(PHS_STATUSMESSAGE* phsPtr, PHS_STATUSMESSAGE_FLAG* phsFlgPtr)
{
}

/*STOP_INTERFACE_MESSAGE*/
void STOP_INTERFACE_MESSAGE_PrepareSend(PHS_STOPINTERFACEMESSAGE* phsPtr, PHS_STOPINTERFACEMESSAGE_FLAG* phsFlgPtr)
{
}

/*TL_ONLY_MESSAGE_IO2LCU*/
void TL_ONLY_MESSAGE_IO2LCU_PrepareSend(PHS_TLONLYMESSAGEIO2LCU* phsPtr, PHS_TLONLYMESSAGEIO2LCU_FLAG* phsFlgPtr)
{
}

/*TL_ONLY_MESSAGE_LCU2IO*/
void TL_ONLY_MESSAGE_LCU2IO_PrepareSend(PHS_TLONLYMESSAGELCU2IO* phsPtr, PHS_TLONLYMESSAGELCU2IO_FLAG* phsFlgPtr)
{
}



#ifdef __cplusplus
}
#endif

#endif 
