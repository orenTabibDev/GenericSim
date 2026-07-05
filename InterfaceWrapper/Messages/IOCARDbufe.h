/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDbufe.h                                                                                                                    */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  External declaration of the I/O buffers related to IOCARD CSCI                                                                  */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDbufe_h
#define IOCARDbufe_h

#include "typelib.h"

#ifdef __cplusplus
extern "C" {
#endif

extern AdiUInt8                                HdrOnlyMessageIo2lcu[];
extern AdiUInt8                                HdrOnlyMessageLcu2io[];
extern AdiUInt8                                KeepAliveMessage[];
extern AdiUInt8                                LogDataMessage[];
extern AdiUInt8                                NonceMessage[];
extern AdiUInt8                                RequestStatusMessage[];
extern AdiUInt8                                SerialIo2lcuMessage[];
extern AdiUInt8                                SerialLcu2ioMessage[];
extern AdiUInt8                                SetDiscretesMessage[];
extern AdiUInt8                                SetSystemStateMessage[];
extern AdiUInt8                                StartInterfaceMessage[];
extern AdiUInt8                                StatusMessage[];
extern AdiUInt8                                StopInterfaceMessage[];
extern AdiUInt8                                TlOnlyMessageIo2lcu[];
extern AdiUInt8                                TlOnlyMessageLcu2io[];


#ifdef __cplusplus
}
#endif

#endif 
