/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDbufplen.h                                                                                                                 */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Public declaration of the I/O length buffers related to IOCARD CSCI                                                             */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDbufplen_h
#define IOCARDbufplen_h


#ifdef __cplusplus
extern "C" {
#endif

#define HdrOnlyMessageIo2lcu_LEN               24
#define HdrOnlyMessageLcu2io_LEN               24
#define KeepAliveMessage_LEN                   28
#define LogDataMessage_LEN                     1064
#define NonceMessage_LEN                       36
#define RequestStatusMessage_LEN               1064
#define SerialIo2lcuMessage_LEN                1060
#define SerialLcu2ioMessage_LEN                1060
#define SetDiscretesMessage_LEN                541
#define SetSystemStateMessage_LEN              32
#define StartInterfaceMessage_LEN              56
#define StatusMessage_LEN                      11764
#define StopInterfaceMessage_LEN               32
#define TlOnlyMessageIo2lcu_LEN                4
#define TlOnlyMessageLcu2io_LEN                4


#ifdef __cplusplus
}
#endif

#endif 
