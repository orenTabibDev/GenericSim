/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDEnumToString.h                                                                                                            */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Enumerated Types to String declarations                                                                                         */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDEnumToString_h
#define IOCARDEnumToString_h

#include "IOCARDenum.h"
#include "typelib.h"

#ifdef __cplusplus
extern "C" {
#endif

//Conversion for numeric types to string
void Integer_ToString(AdiInt64 theValue, AdiString theString, AdiInt maxLength);
//Uses default precision 3 places after decimal point.
void Real_ToString(AdiFloat64 theValue, AdiString theString, AdiInt maxLength);
void Real_ToStringPrecision(AdiFloat64 theValue, AdiString theString, AdiInt maxLength, AdiInt precision);

void E_BOOLEAN_ToString(E_BOOLEAN theValue, AdiString theString, AdiInt maxLength);
void E_CBIT_STATUS_ToString(E_CBIT_STATUS theValue, AdiString theString, AdiInt maxLength);
void E_DISCRETE_ID_ToString(E_DISCRETE_ID theValue, AdiString theString, AdiInt maxLength);
void E_ERROR_ID_ToString(E_ERROR_ID theValue, AdiString theString, AdiInt maxLength);
void E_LOG_ID_ToString(E_LOG_ID theValue, AdiString theString, AdiInt maxLength);
void E_LOG_LEVEL_ToString(E_LOG_LEVEL theValue, AdiString theString, AdiInt maxLength);
void E_MESSAGE_TYPE_ToString(E_MESSAGE_TYPE theValue, AdiString theString, AdiInt maxLength);
void E_PARITY_ToString(E_PARITY theValue, AdiString theString, AdiInt maxLength);
void E_REGISTRATION_STATUS_ToString(E_REGISTRATION_STATUS theValue, AdiString theString, AdiInt maxLength);
void E_UART_ID_ToString(E_UART_ID theValue, AdiString theString, AdiInt maxLength);
void E_UNIT_ID_ToString(E_UNIT_ID theValue, AdiString theString, AdiInt maxLength);
void E_UNIT_STATE_ToString(E_UNIT_STATE theValue, AdiString theString, AdiInt maxLength);


#ifdef __cplusplus
}
#endif

#endif 
