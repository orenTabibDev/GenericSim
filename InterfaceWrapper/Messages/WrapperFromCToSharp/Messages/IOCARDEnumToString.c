/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDEnumToString.c                                                                                                            */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Enumerated Types to String definitions                                                                                          */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDEnumToString_c
#define IOCARDEnumToString_c

#include "IOCARDEnumToString.h"
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include "typelib.h"


//Conversion for numeric types to string
void Integer_ToString(AdiInt64 theValue, AdiString theString, AdiInt maxLength)
{
	_i64toa_s(theValue,theString,maxLength,10);
}

void Real_ToString(AdiFloat64 theValue, AdiString theString, AdiInt maxLength)
{
	Real_ToStringPrecision(theValue, theString, maxLength, 3);
}

void Real_ToStringPrecision(AdiFloat64 theValue, AdiString theString, AdiInt maxLength, AdiInt precision)
{
	AdiInt8 formatString[10];
	sprintf_s(formatString,10,"%c.%df",'%',precision);
	sprintf_s(theString,maxLength,formatString,theValue);
}

void E_BOOLEAN_ToString(E_BOOLEAN theValue, AdiString theString, AdiInt maxLength)
{
	switch (theValue)
	{
		case E_BOOLEAN_FALSE:
			memcpy_s(theString, maxLength, "FALSE", 6);
			break;
		case E_BOOLEAN_TRUE:
			memcpy_s(theString, maxLength, "TRUE", 5);
			break;
		default: break;
	}
}
void E_CBIT_STATUS_ToString(E_CBIT_STATUS theValue, AdiString theString, AdiInt maxLength)
{
	switch (theValue)
	{
		case E_CBIT_STATUS_CBIT_PASS:
			memcpy_s(theString, maxLength, "CBIT_PASS", 10);
			break;
		case E_CBIT_STATUS_CBIT_FAIL:
			memcpy_s(theString, maxLength, "CBIT_FAIL", 10);
			break;
		case E_CBIT_STATUS_CBIT_FATAL:
			memcpy_s(theString, maxLength, "CBIT_FATAL", 11);
			break;
		default: break;
	}
}
void E_DISCRETE_ID_ToString(E_DISCRETE_ID theValue, AdiString theString, AdiInt maxLength)
{
	switch (theValue)
	{
		case E_DISCRETE_ID_MU_RTF_CMD:
			memcpy_s(theString, maxLength, "MU_RTF_CMD", 11);
			break;
		case E_DISCRETE_ID_MU2FIB_SW_ILLUM_CMD:
			memcpy_s(theString, maxLength, "MU2FIB_SW_ILLUM_CMD", 20);
			break;
		case E_DISCRETE_ID_MU2IO_MSU1_POWER_ON:
			memcpy_s(theString, maxLength, "MU2IO_MSU1_POWER_ON", 20);
			break;
		case E_DISCRETE_ID_MU2IO_MSU2_POWER_ON:
			memcpy_s(theString, maxLength, "MU2IO_MSU2_POWER_ON", 20);
			break;
		case E_DISCRETE_ID_MU2IO_MSU3_POWER_ON:
			memcpy_s(theString, maxLength, "MU2IO_MSU3_POWER_ON", 20);
			break;
		case E_DISCRETE_ID_MU2IO_MSU4_POWER_ON:
			memcpy_s(theString, maxLength, "MU2IO_MSU4_POWER_ON", 20);
			break;
		case E_DISCRETE_ID_SSTU_PWR_ON_CMD:
			memcpy_s(theString, maxLength, "SSTU_PWR_ON_CMD", 16);
			break;
		case E_DISCRETE_ID_LPS_PWR_ON_CMD:
			memcpy_s(theString, maxLength, "LPS_PWR_ON_CMD", 15);
			break;
		case E_DISCRETE_ID_STRU_PWR_ON_CMD:
			memcpy_s(theString, maxLength, "STRU_PWR_ON_CMD", 16);
			break;
		case E_DISCRETE_ID_MU2MSU1_MSL1_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU1_MSL1_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU1_MSL2_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU1_MSL2_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU1_MSL3_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU1_MSL3_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU1_MSL4_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU1_MSL4_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU1_MSL1_PTF:
			memcpy_s(theString, maxLength, "MU2MSU1_MSL1_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU1_MSL2_PTF:
			memcpy_s(theString, maxLength, "MU2MSU1_MSL2_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU1_MSL3_PTF:
			memcpy_s(theString, maxLength, "MU2MSU1_MSL3_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU1_MSL4_PTF:
			memcpy_s(theString, maxLength, "MU2MSU1_MSL4_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU1_F_CMD:
			memcpy_s(theString, maxLength, "MU2MSU1_F_CMD", 14);
			break;
		case E_DISCRETE_ID_MU2MSU1_BTLS_CMD:
			memcpy_s(theString, maxLength, "MU2MSU1_BTLS_CMD", 17);
			break;
		case E_DISCRETE_ID_MU2MSU2_MSL1_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU2_MSL1_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU2_MSL2_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU2_MSL2_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU2_MSL3_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU2_MSL3_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU2_MSL4_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU2_MSL4_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU2_MSL1_PTF:
			memcpy_s(theString, maxLength, "MU2MSU2_MSL1_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU2_MSL2_PTF:
			memcpy_s(theString, maxLength, "MU2MSU2_MSL2_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU2_MSL3_PTF:
			memcpy_s(theString, maxLength, "MU2MSU2_MSL3_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU2_MSL4_PTF:
			memcpy_s(theString, maxLength, "MU2MSU2_MSL4_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU2_F_CMD:
			memcpy_s(theString, maxLength, "MU2MSU2_F_CMD", 14);
			break;
		case E_DISCRETE_ID_MU2MSU2_BTLS_CMD:
			memcpy_s(theString, maxLength, "MU2MSU2_BTLS_CMD", 17);
			break;
		case E_DISCRETE_ID_MU2MSU3_MSL1_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU3_MSL1_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU3_MSL2_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU3_MSL2_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU3_MSL3_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU3_MSL3_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU3_MSL4_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU3_MSL4_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU3_MSL1_PTF:
			memcpy_s(theString, maxLength, "MU2MSU3_MSL1_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU3_MSL2_PTF:
			memcpy_s(theString, maxLength, "MU2MSU3_MSL2_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU3_MSL3_PTF:
			memcpy_s(theString, maxLength, "MU2MSU3_MSL3_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU3_MSL4_PTF:
			memcpy_s(theString, maxLength, "MU2MSU3_MSL4_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU3_F_CMD:
			memcpy_s(theString, maxLength, "MU2MSU3_F_CMD", 14);
			break;
		case E_DISCRETE_ID_MU2MSU3_BTLS_CMD:
			memcpy_s(theString, maxLength, "MU2MSU3_BTLS_CMD", 17);
			break;
		case E_DISCRETE_ID_MU2MSU4_MSL1_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU4_MSL1_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU4_MSL2_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU4_MSL2_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU4_MSL3_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU4_MSL3_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU4_MSL4_F_EN:
			memcpy_s(theString, maxLength, "MU2MSU4_MSL4_F_EN", 18);
			break;
		case E_DISCRETE_ID_MU2MSU4_MSL1_PTF:
			memcpy_s(theString, maxLength, "MU2MSU4_MSL1_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU4_MSL2_PTF:
			memcpy_s(theString, maxLength, "MU2MSU4_MSL2_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU4_MSL3_PTF:
			memcpy_s(theString, maxLength, "MU2MSU4_MSL3_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU4_MSL4_PTF:
			memcpy_s(theString, maxLength, "MU2MSU4_MSL4_PTF", 17);
			break;
		case E_DISCRETE_ID_MU2MSU4_F_CMD:
			memcpy_s(theString, maxLength, "MU2MSU4_F_CMD", 14);
			break;
		case E_DISCRETE_ID_MU2MSU4_BTLS_CMD:
			memcpy_s(theString, maxLength, "MU2MSU4_BTLS_CMD", 17);
			break;
		case E_DISCRETE_ID_CAST_REF:
			memcpy_s(theString, maxLength, "CAST_REF", 9);
			break;
		case E_DISCRETE_ID_TEST_REF:
			memcpy_s(theString, maxLength, "TEST_REF", 9);
			break;
		case E_DISCRETE_ID_MISN_REF:
			memcpy_s(theString, maxLength, "MISN_REF", 9);
			break;
		case E_DISCRETE_ID_SAFE_REF:
			memcpy_s(theString, maxLength, "SAFE_REF", 9);
			break;
		case E_DISCRETE_ID_FIB_FKEY_ARM:
			memcpy_s(theString, maxLength, "FIB_FKEY_ARM", 13);
			break;
		case E_DISCRETE_ID_FIB_F_PB:
			memcpy_s(theString, maxLength, "FIB_F_PB", 9);
			break;
		case E_DISCRETE_ID_SSTU_INHIBIT_CTRL:
			memcpy_s(theString, maxLength, "SSTU_INHIBIT_CTRL", 18);
			break;
		case E_DISCRETE_ID_SSTU_VOK_IND:
			memcpy_s(theString, maxLength, "SSTU_VOK_IND", 13);
			break;
		case E_DISCRETE_ID_LPS_PWR_INHIBIT:
			memcpy_s(theString, maxLength, "LPS_PWR_INHIBIT", 16);
			break;
		case E_DISCRETE_ID_LPS_VOK_IND:
			memcpy_s(theString, maxLength, "LPS_VOK_IND", 12);
			break;
		case E_DISCRETE_ID_STRU_VOK_IND:
			memcpy_s(theString, maxLength, "STRU_VOK_IND", 13);
			break;
		case E_DISCRETE_ID_STRU_PWR_INHIBIT:
			memcpy_s(theString, maxLength, "STRU_PWR_INHIBIT", 17);
			break;
		case E_DISCRETE_ID_LPS_CB_IND:
			memcpy_s(theString, maxLength, "LPS_CB_IND", 11);
			break;
		case E_DISCRETE_ID_STRU_TX_ON_IND:
			memcpy_s(theString, maxLength, "STRU_TX_ON_IND", 15);
			break;
		case E_DISCRETE_ID_MU_ON_IND:
			memcpy_s(theString, maxLength, "MU_ON_IND", 10);
			break;
		case E_DISCRETE_ID_MSU1_CAST_MODE:
			memcpy_s(theString, maxLength, "MSU1_CAST_MODE", 15);
			break;
		case E_DISCRETE_ID_MSU1_MISN_MODE:
			memcpy_s(theString, maxLength, "MSU1_MISN_MODE", 15);
			break;
		case E_DISCRETE_ID_MSU1_TST_MODE:
			memcpy_s(theString, maxLength, "MSU1_TST_MODE", 14);
			break;
		case E_DISCRETE_ID_MSU1_CB_IND:
			memcpy_s(theString, maxLength, "MSU1_CB_IND", 12);
			break;
		case E_DISCRETE_ID_MSU1_BTLS_CMD:
			memcpy_s(theString, maxLength, "MSU1_BTLS_CMD", 14);
			break;
		case E_DISCRETE_ID_MSU1_OVER_TEMP_IND:
			memcpy_s(theString, maxLength, "MSU1_OVER_TEMP_IND", 19);
			break;
		case E_DISCRETE_ID_MSU1_OK_IND:
			memcpy_s(theString, maxLength, "MSU1_OK_IND", 12);
			break;
		case E_DISCRETE_ID_MSU1_ON_IND:
			memcpy_s(theString, maxLength, "MSU1_ON_IND", 12);
			break;
		case E_DISCRETE_ID_MSU1_VOK_IND:
			memcpy_s(theString, maxLength, "MSU1_VOK_IND", 13);
			break;
		case E_DISCRETE_ID_MSU1_POWER_ON_OR_RET:
			memcpy_s(theString, maxLength, "MSU1_POWER_ON_OR_RET", 21);
			break;
		case E_DISCRETE_ID_MSU1_HOLD_ON:
			memcpy_s(theString, maxLength, "MSU1_HOLD_ON", 13);
			break;
		case E_DISCRETE_ID_MSU1_MSL1_F_EN:
			memcpy_s(theString, maxLength, "MSU1_MSL1_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU1_MSL2_F_EN:
			memcpy_s(theString, maxLength, "MSU1_MSL2_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU1_MSL3_F_EN:
			memcpy_s(theString, maxLength, "MSU1_MSL3_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU1_MSL4_F_EN:
			memcpy_s(theString, maxLength, "MSU1_MSL4_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU1_MSL1_PTF:
			memcpy_s(theString, maxLength, "MSU1_MSL1_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU1_MSL2_PTF:
			memcpy_s(theString, maxLength, "MSU1_MSL2_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU1_MSL3_PTF:
			memcpy_s(theString, maxLength, "MSU1_MSL3_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU1_MSL4_PTF:
			memcpy_s(theString, maxLength, "MSU1_MSL4_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU1_MLS1_CMD_RET:
			memcpy_s(theString, maxLength, "MSU1_MLS1_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU1_MLS2_CMD_RET:
			memcpy_s(theString, maxLength, "MSU1_MLS2_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU1_MLS3_CMD_RET:
			memcpy_s(theString, maxLength, "MSU1_MLS3_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU1_MLS4_CMD_RET:
			memcpy_s(theString, maxLength, "MSU1_MLS4_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU1_F_CMD:
			memcpy_s(theString, maxLength, "MSU1_F_CMD", 11);
			break;
		case E_DISCRETE_ID_MSU1_DIS_F:
			memcpy_s(theString, maxLength, "MSU1_DIS_F", 11);
			break;
		case E_DISCRETE_ID_MSU1_2_SEC_DELAY:
			memcpy_s(theString, maxLength, "MSU1_2_SEC_DELAY", 17);
			break;
		case E_DISCRETE_ID_MSU2_CAST_MODE:
			memcpy_s(theString, maxLength, "MSU2_CAST_MODE", 15);
			break;
		case E_DISCRETE_ID_MSU2_MISN_MODE:
			memcpy_s(theString, maxLength, "MSU2_MISN_MODE", 15);
			break;
		case E_DISCRETE_ID_MSU2_TST_MODE:
			memcpy_s(theString, maxLength, "MSU2_TST_MODE", 14);
			break;
		case E_DISCRETE_ID_MSU2_CB_IND:
			memcpy_s(theString, maxLength, "MSU2_CB_IND", 12);
			break;
		case E_DISCRETE_ID_MSU2_BTLS_CMD:
			memcpy_s(theString, maxLength, "MSU2_BTLS_CMD", 14);
			break;
		case E_DISCRETE_ID_MSU2_OVER_TEMP_IND:
			memcpy_s(theString, maxLength, "MSU2_OVER_TEMP_IND", 19);
			break;
		case E_DISCRETE_ID_MSU2_OK_IND:
			memcpy_s(theString, maxLength, "MSU2_OK_IND", 12);
			break;
		case E_DISCRETE_ID_MSU2_ON_IND:
			memcpy_s(theString, maxLength, "MSU2_ON_IND", 12);
			break;
		case E_DISCRETE_ID_MSU2_VOK_IND:
			memcpy_s(theString, maxLength, "MSU2_VOK_IND", 13);
			break;
		case E_DISCRETE_ID_MSU2_POWER_ON_OR_RET:
			memcpy_s(theString, maxLength, "MSU2_POWER_ON_OR_RET", 21);
			break;
		case E_DISCRETE_ID_MSU2_HOLD_ON:
			memcpy_s(theString, maxLength, "MSU2_HOLD_ON", 13);
			break;
		case E_DISCRETE_ID_MSU2_MSL1_F_EN:
			memcpy_s(theString, maxLength, "MSU2_MSL1_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU2_MSL2_F_EN:
			memcpy_s(theString, maxLength, "MSU2_MSL2_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU2_MSL3_F_EN:
			memcpy_s(theString, maxLength, "MSU2_MSL3_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU2_MSL4_F_EN:
			memcpy_s(theString, maxLength, "MSU2_MSL4_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU2_MSL1_PTF:
			memcpy_s(theString, maxLength, "MSU2_MSL1_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU2_MSL2_PTF:
			memcpy_s(theString, maxLength, "MSU2_MSL2_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU2_MSL3_PTF:
			memcpy_s(theString, maxLength, "MSU2_MSL3_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU2_MSL4_PTF:
			memcpy_s(theString, maxLength, "MSU2_MSL4_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU2_MLS1_CMD_RET:
			memcpy_s(theString, maxLength, "MSU2_MLS1_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU2_MLS2_CMD_RET:
			memcpy_s(theString, maxLength, "MSU2_MLS2_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU2_MLS3_CMD_RET:
			memcpy_s(theString, maxLength, "MSU2_MLS3_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU2_MLS4_CMD_RET:
			memcpy_s(theString, maxLength, "MSU2_MLS4_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU2_F_CMD:
			memcpy_s(theString, maxLength, "MSU2_F_CMD", 11);
			break;
		case E_DISCRETE_ID_MSU2_DIS_F:
			memcpy_s(theString, maxLength, "MSU2_DIS_F", 11);
			break;
		case E_DISCRETE_ID_MSU2_2_SEC_DELAY:
			memcpy_s(theString, maxLength, "MSU2_2_SEC_DELAY", 17);
			break;
		case E_DISCRETE_ID_MSU3_CAST_MODE:
			memcpy_s(theString, maxLength, "MSU3_CAST_MODE", 15);
			break;
		case E_DISCRETE_ID_MSU3_MISN_MODE:
			memcpy_s(theString, maxLength, "MSU3_MISN_MODE", 15);
			break;
		case E_DISCRETE_ID_MSU3_TST_MODE:
			memcpy_s(theString, maxLength, "MSU3_TST_MODE", 14);
			break;
		case E_DISCRETE_ID_MSU3_CB_IND:
			memcpy_s(theString, maxLength, "MSU3_CB_IND", 12);
			break;
		case E_DISCRETE_ID_MSU3_BTLS_CMD:
			memcpy_s(theString, maxLength, "MSU3_BTLS_CMD", 14);
			break;
		case E_DISCRETE_ID_MSU3_OVER_TEMP_IND:
			memcpy_s(theString, maxLength, "MSU3_OVER_TEMP_IND", 19);
			break;
		case E_DISCRETE_ID_MSU3_OK_IND:
			memcpy_s(theString, maxLength, "MSU3_OK_IND", 12);
			break;
		case E_DISCRETE_ID_MSU3_ON_IND:
			memcpy_s(theString, maxLength, "MSU3_ON_IND", 12);
			break;
		case E_DISCRETE_ID_MSU3_VOK_IND:
			memcpy_s(theString, maxLength, "MSU3_VOK_IND", 13);
			break;
		case E_DISCRETE_ID_MSU3_POWER_ON_OR_RET:
			memcpy_s(theString, maxLength, "MSU3_POWER_ON_OR_RET", 21);
			break;
		case E_DISCRETE_ID_MSU3_HOLD_ON:
			memcpy_s(theString, maxLength, "MSU3_HOLD_ON", 13);
			break;
		case E_DISCRETE_ID_MSU3_MSL1_F_EN:
			memcpy_s(theString, maxLength, "MSU3_MSL1_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU3_MSL2_F_EN:
			memcpy_s(theString, maxLength, "MSU3_MSL2_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU3_MSL3_F_EN:
			memcpy_s(theString, maxLength, "MSU3_MSL3_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU3_MSL4_F_EN:
			memcpy_s(theString, maxLength, "MSU3_MSL4_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU3_MSL1_PTF:
			memcpy_s(theString, maxLength, "MSU3_MSL1_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU3_MSL2_PTF:
			memcpy_s(theString, maxLength, "MSU3_MSL2_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU3_MSL3_PTF:
			memcpy_s(theString, maxLength, "MSU3_MSL3_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU3_MSL4_PTF:
			memcpy_s(theString, maxLength, "MSU3_MSL4_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU3_MLS1_CMD_RET:
			memcpy_s(theString, maxLength, "MSU3_MLS1_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU3_MLS2_CMD_RET:
			memcpy_s(theString, maxLength, "MSU3_MLS2_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU3_MLS3_CMD_RET:
			memcpy_s(theString, maxLength, "MSU3_MLS3_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU3_MLS4_CMD_RET:
			memcpy_s(theString, maxLength, "MSU3_MLS4_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU3_F_CMD:
			memcpy_s(theString, maxLength, "MSU3_F_CMD", 11);
			break;
		case E_DISCRETE_ID_MSU3_DIS_F:
			memcpy_s(theString, maxLength, "MSU3_DIS_F", 11);
			break;
		case E_DISCRETE_ID_MSU3_2_SEC_DELAY:
			memcpy_s(theString, maxLength, "MSU3_2_SEC_DELAY", 17);
			break;
		case E_DISCRETE_ID_MSU4_CAST_MODE:
			memcpy_s(theString, maxLength, "MSU4_CAST_MODE", 15);
			break;
		case E_DISCRETE_ID_MSU4_MISN_MODE:
			memcpy_s(theString, maxLength, "MSU4_MISN_MODE", 15);
			break;
		case E_DISCRETE_ID_MSU4_TST_MODE:
			memcpy_s(theString, maxLength, "MSU4_TST_MODE", 14);
			break;
		case E_DISCRETE_ID_MSU4_CB_IND:
			memcpy_s(theString, maxLength, "MSU4_CB_IND", 12);
			break;
		case E_DISCRETE_ID_MSU4_BTLS_CMD:
			memcpy_s(theString, maxLength, "MSU4_BTLS_CMD", 14);
			break;
		case E_DISCRETE_ID_MSU4_OVER_TEMP_IND:
			memcpy_s(theString, maxLength, "MSU4_OVER_TEMP_IND", 19);
			break;
		case E_DISCRETE_ID_MSU4_OK_IND:
			memcpy_s(theString, maxLength, "MSU4_OK_IND", 12);
			break;
		case E_DISCRETE_ID_MSU4_ON_IND:
			memcpy_s(theString, maxLength, "MSU4_ON_IND", 12);
			break;
		case E_DISCRETE_ID_MSU4_VOK_IND:
			memcpy_s(theString, maxLength, "MSU4_VOK_IND", 13);
			break;
		case E_DISCRETE_ID_MSU4_POWER_ON_OR_RET:
			memcpy_s(theString, maxLength, "MSU4_POWER_ON_OR_RET", 21);
			break;
		case E_DISCRETE_ID_MSU4_HOLD_ON:
			memcpy_s(theString, maxLength, "MSU4_HOLD_ON", 13);
			break;
		case E_DISCRETE_ID_MSU4_MSL1_F_EN:
			memcpy_s(theString, maxLength, "MSU4_MSL1_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU4_MSL2_F_EN:
			memcpy_s(theString, maxLength, "MSU4_MSL2_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU4_MSL3_F_EN:
			memcpy_s(theString, maxLength, "MSU4_MSL3_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU4_MSL4_F_EN:
			memcpy_s(theString, maxLength, "MSU4_MSL4_F_EN", 15);
			break;
		case E_DISCRETE_ID_MSU4_MSL1_PTF:
			memcpy_s(theString, maxLength, "MSU4_MSL1_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU4_MSL2_PTF:
			memcpy_s(theString, maxLength, "MSU4_MSL2_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU4_MSL3_PTF:
			memcpy_s(theString, maxLength, "MSU4_MSL3_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU4_MSL4_PTF:
			memcpy_s(theString, maxLength, "MSU4_MSL4_PTF", 14);
			break;
		case E_DISCRETE_ID_MSU4_MLS1_CMD_RET:
			memcpy_s(theString, maxLength, "MSU4_MLS1_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU4_MLS2_CMD_RET:
			memcpy_s(theString, maxLength, "MSU4_MLS2_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU4_MLS3_CMD_RET:
			memcpy_s(theString, maxLength, "MSU4_MLS3_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU4_MLS4_CMD_RET:
			memcpy_s(theString, maxLength, "MSU4_MLS4_CMD_RET", 18);
			break;
		case E_DISCRETE_ID_MSU4_F_CMD:
			memcpy_s(theString, maxLength, "MSU4_F_CMD", 11);
			break;
		case E_DISCRETE_ID_MSU4_DIS_F:
			memcpy_s(theString, maxLength, "MSU4_DIS_F", 11);
			break;
		case E_DISCRETE_ID_MSU4_2_SEC_DELAY:
			memcpy_s(theString, maxLength, "MSU4_2_SEC_DELAY", 17);
			break;
		case E_DISCRETE_ID_FIB_CAST_MODE:
			memcpy_s(theString, maxLength, "FIB_CAST_MODE", 14);
			break;
		case E_DISCRETE_ID_FIB_TEST_MODE:
			memcpy_s(theString, maxLength, "FIB_TEST_MODE", 14);
			break;
		case E_DISCRETE_ID_FIB_MISN_MODE:
			memcpy_s(theString, maxLength, "FIB_MISN_MODE", 14);
			break;
		case E_DISCRETE_ID_FIB_SAFE_MODE:
			memcpy_s(theString, maxLength, "FIB_SAFE_MODE", 14);
			break;
		case E_DISCRETE_ID_FIB_PTF:
			memcpy_s(theString, maxLength, "FIB_PTF", 8);
			break;
		case E_DISCRETE_ID_FIB_F_EN:
			memcpy_s(theString, maxLength, "FIB_F_EN", 9);
			break;
		case E_DISCRETE_ID_FIB2LU_PERMIT1:
			memcpy_s(theString, maxLength, "FIB2LU_PERMIT1", 15);
			break;
		case E_DISCRETE_ID_FIB2LU_PERMIT2:
			memcpy_s(theString, maxLength, "FIB2LU_PERMIT2", 15);
			break;
		case E_DISCRETE_ID_FIB2LU_PERMIT3:
			memcpy_s(theString, maxLength, "FIB2LU_PERMIT3", 15);
			break;
		case E_DISCRETE_ID_FIB2LU_PERMIT4:
			memcpy_s(theString, maxLength, "FIB2LU_PERMIT4", 15);
			break;
		case E_DISCRETE_ID_LU2FIB_BLOCK1:
			memcpy_s(theString, maxLength, "LU2FIB_BLOCK1", 14);
			break;
		case E_DISCRETE_ID_LU2FIB_BLOCK2:
			memcpy_s(theString, maxLength, "LU2FIB_BLOCK2", 14);
			break;
		case E_DISCRETE_ID_LU2FIB_BLOCK3:
			memcpy_s(theString, maxLength, "LU2FIB_BLOCK3", 14);
			break;
		case E_DISCRETE_ID_LU2FIB_BLOCK4:
			memcpy_s(theString, maxLength, "LU2FIB_BLOCK4", 14);
			break;
		default: break;
	}
}
void E_ERROR_ID_ToString(E_ERROR_ID theValue, AdiString theString, AdiInt maxLength)
{
	switch (theValue)
	{
		case E_ERROR_ID_SUCCESS:
			memcpy_s(theString, maxLength, "SUCCESS", 8);
			break;
		case E_ERROR_ID_INCORRECT_ID:
			memcpy_s(theString, maxLength, "INCORRECT_ID", 13);
			break;
		case E_ERROR_ID_INCORRECT_LENGTH:
			memcpy_s(theString, maxLength, "INCORRECT_LENGTH", 17);
			break;
		case E_ERROR_ID_INCORRECT_CRC:
			memcpy_s(theString, maxLength, "INCORRECT_CRC", 14);
			break;
		case E_ERROR_ID_EXECUTION_ERROR:
			memcpy_s(theString, maxLength, "EXECUTION_ERROR", 16);
			break;
		case E_ERROR_ID_REPEATED_SEQ_COUNT:
			memcpy_s(theString, maxLength, "REPEATED_SEQ_COUNT", 19);
			break;
		case E_ERROR_ID_INCORRECT_NONCE_VAL:
			memcpy_s(theString, maxLength, "INCORRECT_NONCE_VAL", 20);
			break;
		default: break;
	}
}
void E_LOG_ID_ToString(E_LOG_ID theValue, AdiString theString, AdiInt maxLength)
{
	switch (theValue)
	{
		case E_LOG_ID_UNDEFINED_1:
			memcpy_s(theString, maxLength, "UNDEFINED_1", 12);
			break;
		case E_LOG_ID_UNDEFINED_2:
			memcpy_s(theString, maxLength, "UNDEFINED_2", 12);
			break;
		case E_LOG_ID_UNDEFINED_3:
			memcpy_s(theString, maxLength, "UNDEFINED_3", 12);
			break;
		default: break;
	}
}
void E_LOG_LEVEL_ToString(E_LOG_LEVEL theValue, AdiString theString, AdiInt maxLength)
{
	switch (theValue)
	{
		case E_LOG_LEVEL_NOT_ASSIGNED:
			memcpy_s(theString, maxLength, "NOT_ASSIGNED", 13);
			break;
		case E_LOG_LEVEL_VERBOSE:
			memcpy_s(theString, maxLength, "VERBOSE", 8);
			break;
		case E_LOG_LEVEL_DEBUG:
			memcpy_s(theString, maxLength, "DEBUG", 6);
			break;
		case E_LOG_LEVEL_WARN:
			memcpy_s(theString, maxLength, "WARN", 5);
			break;
		case E_LOG_LEVEL_ERROR:
			memcpy_s(theString, maxLength, "ERROR", 6);
			break;
		case E_LOG_LEVEL_CRIT_ERR:
			memcpy_s(theString, maxLength, "CRIT_ERR", 9);
			break;
		default: break;
	}
}
void E_MESSAGE_TYPE_ToString(E_MESSAGE_TYPE theValue, AdiString theString, AdiInt maxLength)
{
	switch (theValue)
	{
		case E_MESSAGE_TYPE_NOT_ASSIGNED:
			memcpy_s(theString, maxLength, "NOT_ASSIGNED", 13);
			break;
		case E_MESSAGE_TYPE_SET_DISCRETES:
			memcpy_s(theString, maxLength, "SET_DISCRETES", 14);
			break;
		case E_MESSAGE_TYPE_SERIAL_LCU2IO:
			memcpy_s(theString, maxLength, "SERIAL_LCU2IO", 14);
			break;
		case E_MESSAGE_TYPE_SET_SYSTEM_STATE:
			memcpy_s(theString, maxLength, "SET_SYSTEM_STATE", 17);
			break;
		case E_MESSAGE_TYPE_PROGRAM_DATA:
			memcpy_s(theString, maxLength, "PROGRAM_DATA", 13);
			break;
		case E_MESSAGE_TYPE_START_UART_INTERFACE:
			memcpy_s(theString, maxLength, "START_UART_INTERFACE", 21);
			break;
		case E_MESSAGE_TYPE_STOP_UART_INTERFACE:
			memcpy_s(theString, maxLength, "STOP_UART_INTERFACE", 20);
			break;
		case E_MESSAGE_TYPE_KEEP_ALIVE:
			memcpy_s(theString, maxLength, "KEEP_ALIVE", 11);
			break;
		case E_MESSAGE_TYPE_REQUEST_STATUS:
			memcpy_s(theString, maxLength, "REQUEST_STATUS", 15);
			break;
		case E_MESSAGE_TYPE_MONITORED_DATA:
			memcpy_s(theString, maxLength, "MONITORED_DATA", 15);
			break;
		case E_MESSAGE_TYPE_SERIAL_IO2LCU:
			memcpy_s(theString, maxLength, "SERIAL_IO2LCU", 14);
			break;
		case E_MESSAGE_TYPE_LOG_OR_ERROR:
			memcpy_s(theString, maxLength, "LOG_OR_ERROR", 13);
			break;
		case E_MESSAGE_TYPE_NONCE:
			memcpy_s(theString, maxLength, "NONCE", 6);
			break;
		default: break;
	}
}
void E_PARITY_ToString(E_PARITY theValue, AdiString theString, AdiInt maxLength)
{
	switch (theValue)
	{
		case E_PARITY_OFF:
			memcpy_s(theString, maxLength, "OFF", 4);
			break;
		case E_PARITY_ODD:
			memcpy_s(theString, maxLength, "ODD", 4);
			break;
		case E_PARITY_EVEN:
			memcpy_s(theString, maxLength, "EVEN", 5);
			break;
		default: break;
	}
}
void E_REGISTRATION_STATUS_ToString(E_REGISTRATION_STATUS theValue, AdiString theString, AdiInt maxLength)
{
	switch (theValue)
	{
		case E_REGISTRATION_STATUS_PBIT_PASS:
			memcpy_s(theString, maxLength, "PBIT_PASS", 10);
			break;
		case E_REGISTRATION_STATUS_PBIT_FAIL:
			memcpy_s(theString, maxLength, "PBIT_FAIL", 10);
			break;
		case E_REGISTRATION_STATUS_PBIT_FATAL:
			memcpy_s(theString, maxLength, "PBIT_FATAL", 11);
			break;
		case E_REGISTRATION_STATUS_NOT_REGISTERED:
			memcpy_s(theString, maxLength, "NOT_REGISTERED", 15);
			break;
		default: break;
	}
}
void E_UART_ID_ToString(E_UART_ID theValue, AdiString theString, AdiInt maxLength)
{
	switch (theValue)
	{
		case E_UART_ID_NOT_ASSIGNED:
			memcpy_s(theString, maxLength, "NOT_ASSIGNED", 13);
			break;
		case E_UART_ID_NAV_10HZ:
			memcpy_s(theString, maxLength, "NAV_10HZ", 9);
			break;
		case E_UART_ID_NAV_100HZ:
			memcpy_s(theString, maxLength, "NAV_100HZ", 10);
			break;
		case E_UART_ID_NAV_NMEA:
			memcpy_s(theString, maxLength, "NAV_NMEA", 9);
			break;
		case E_UART_ID_ANCU_1:
			memcpy_s(theString, maxLength, "ANCU_1", 7);
			break;
		case E_UART_ID_ANCU_2:
			memcpy_s(theString, maxLength, "ANCU_2", 7);
			break;
		default: break;
	}
}
void E_UNIT_ID_ToString(E_UNIT_ID theValue, AdiString theString, AdiInt maxLength)
{
	switch (theValue)
	{
		case E_UNIT_ID_NOT_ASSIGNED:
			memcpy_s(theString, maxLength, "NOT_ASSIGNED", 13);
			break;
		case E_UNIT_ID_IOUNIT:
			memcpy_s(theString, maxLength, "IOUNIT", 7);
			break;
		case E_UNIT_ID_FIB:
			memcpy_s(theString, maxLength, "FIB", 4);
			break;
		case E_UNIT_ID_LUE2D_1:
			memcpy_s(theString, maxLength, "LUE2D_1", 8);
			break;
		case E_UNIT_ID_LUE2D_2:
			memcpy_s(theString, maxLength, "LUE2D_2", 8);
			break;
		case E_UNIT_ID_LUE2D_3:
			memcpy_s(theString, maxLength, "LUE2D_3", 8);
			break;
		case E_UNIT_ID_LUE2D_4:
			memcpy_s(theString, maxLength, "LUE2D_4", 8);
			break;
		default: break;
	}
}
void E_UNIT_STATE_ToString(E_UNIT_STATE theValue, AdiString theString, AdiInt maxLength)
{
	switch (theValue)
	{
		case E_UNIT_STATE_IDLE:
			memcpy_s(theString, maxLength, "IDLE", 5);
			break;
		case E_UNIT_STATE_OPERATIONAL:
			memcpy_s(theString, maxLength, "OPERATIONAL", 12);
			break;
		case E_UNIT_STATE_PROGRAMMING:
			memcpy_s(theString, maxLength, "PROGRAMMING", 12);
			break;
		default: break;
	}
}



#endif 
