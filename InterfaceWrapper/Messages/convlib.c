/************************************************************************
 * File:        convlib.c                                               
 *                                                                      
 * Description: Big Endian <-> Little Endian Converts & data align    
 *                                                                      
 * Procedures:                                                                                              
 *              many...                                                                                     
 *                                                                      
 * Last Updated:  
 *
 *			28-07-13 S.W. - fix compilation errors related to previous changes.
 *			03-12-12 O.S. - add converts from Sign magnitude/Two complemente
 *			30-09-09 E.H. - cosmetics                                                    
 *			26-05-08 N.G. - custom real/double check, align/swap buffer fumctions added                                                    
 *                                                                      
 ************************************************************************/
#include "typelib.h"
#include <stdio.h>
#include <float.h> /* include for Windows-based system */
#include <memory.h>
#include <math.h> /* 06-01-09 */
//#include <basetsd.h> /* 06-01-09 */
//#include "typelib.h"
#include "convlib.h"
#include <stdlib.h>

#ifndef CUSTOM_REAL_CHECKS

#endif

/* All of these functions written for the SPARC & Intel Integers  */
/* MSB FIRST                                                                                    */
/* LSB LAST                                                                                      */

/*AdiInt check_if_real( AdiFloat32 value )
{
   return _finite(value);
}*/

/* 06-01-09 */
// converts from one's complement interface bits representation (of unsigned integer 64) to
// signed integer 64 (8 bytes) two's complement standard representation
// used in convert to physical
AdiInt64 convert_from_ones_complement(AdiUInt64 value, AdiInt maskSize)
{
    // if negative value (msb is 1)
    if (value >= (AdiUInt64)(pow(2, maskSize -1)))
        // convert value from one's complement to two's complement return modified value
        // use bitwise Or to fill msb bits
        return (AdiInt64)(value+1)|(AdiInt64)(pow(2, 64) - pow(2, maskSize -1));
    else
        return (AdiInt64)value;
}

// converts to one's complement interface bits representation (of unsigned integer 64) from
// signed integer 64 (8 bytes) two's complement standard representation
// used in convert to interface
AdiUInt64 convert_to_ones_complement(AdiInt64 value, AdiInt maskSize)
{
    // if negative value (msb is 1)
    if (value < 0)
        // convert value from two's complement to one's complement return modified value (cast to unsigned)
        return (AdiUInt64)(value-1);
    else
        // return value as is (cast to unsigned)
        return (AdiUInt64)value;
}

// converts from two's complement interface bits representation (of unsigned integer 64) to
// signed integer 64 (8 bytes) two's complement standard representation
// used in convert to physical
AdiInt64 convert_from_twos_complement(AdiUInt64 value, AdiInt maskSize)
{
    // if negative value (msb is 1)
    if (value >= power2( maskSize -1))
        // convert value from one's complement to two's complement return modified value
        // use bitwise Or to fill msb bits
        return (AdiInt64)(value - power2( maskSize ));
    else
        return (AdiInt64)value;
}

// converts to two's complement interface bits representation (of unsigned integer 64) from
// signed integer 64 (8 bytes) two's complement standard representation
// used in convert to interface
AdiUInt64 convert_to_twos_complement(AdiInt64 value, AdiInt maskSize)
{
    // if negative value (msb is 1)
    if (value < 0)
        // convert value from two's complement to one's complement return modified value (cast to unsigned)
        return (AdiUInt64)(value-1);
    else
        // return value as is (cast to unsigned)
        return (AdiUInt64)value;
}

// converts from sign magnitude interface bits representation (of unsigned integer 64) to
// signed integer 64 (8 bytes) two's complement standard representation
// used in convert to physical
AdiInt64 convert_from_sign_magnitude(AdiUInt64 value, AdiInt maskSize)
{
		// if negative value (msb is 1)
    if (value >= (AdiUInt64)(pow(2, maskSize -1)))
		{
			// convert negative value 
			AdiUInt64 magnValue = value/2 - value;
			// return value as is (cast to unsigned)
			return (AdiInt64)magnValue;
		}
		else
			// return value as is (cast to unsigned)
			return (AdiInt64)value;
}

// converts to sign magnitude  interface bits representation (of unsigned integer 64) from
// signed integer 64 (8 bytes) two's complement standard representation
// used in convert to interface
AdiUInt64 convert_to_sign_magnitude(AdiInt64 value, AdiInt maskSize)
{
    // if negative value (msb is 1)
    if (value < 0)
		{
			value *= value;
			value = (AdiInt64)sqrt((AdiFloat64)value);
			value *= 2;
      // convert value from sign magnitude to two's complement standard, 
			// return modified value (cast to unsigned)
      return (AdiUInt64)(value);
		}
    else
      // return value as is (cast to unsigned)
      return (AdiUInt64)value;
}
AdiFloat32 get_float_swap( AdiUInt8 * IntfPtr )
{

        union
        {
                AdiFloat32                 FloatValue;
                AdiUInt8   ByteValue[4];
                AdiUInt32  LongValue;
        } FourByteUnion;

        FourByteUnion.ByteValue[3] = *IntfPtr;
        FourByteUnion.ByteValue[2] = *(IntfPtr+1);
        FourByteUnion.ByteValue[1] = *(IntfPtr+2);
        FourByteUnion.ByteValue[0] = *(IntfPtr+3);

        if(FourByteUnion.LongValue)
                return FourByteUnion.FloatValue;
        else    return 0.0;

}


AdiFloat64 get_double_swap( AdiUInt8 * IntfPtr )
{

        union
        {
                AdiFloat64                 DoubleValue;
                AdiUInt8   ByteValue[8];
                AdiUInt32  LongValue;
        } EightByteUnion;

        EightByteUnion.ByteValue[7] = *IntfPtr;
        EightByteUnion.ByteValue[6] = *(IntfPtr+1);
        EightByteUnion.ByteValue[5] = *(IntfPtr+2);
        EightByteUnion.ByteValue[4] = *(IntfPtr+3);
        EightByteUnion.ByteValue[3] = *(IntfPtr+4);
        EightByteUnion.ByteValue[2] = *(IntfPtr+5);
        EightByteUnion.ByteValue[1] = *(IntfPtr+6);
        EightByteUnion.ByteValue[0] = *(IntfPtr+7);


        if(EightByteUnion.LongValue)
                return EightByteUnion.DoubleValue;
        else    return 0.0;

}
AdiInt get_int_swap( AdiUInt8 * IntfPtr )
{
        union
        {
                AdiInt                   IntValue;
                AdiUInt8   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.ByteValue[3] = *IntfPtr;
        FourByteUnion.ByteValue[2] = *(IntfPtr+1);
        FourByteUnion.ByteValue[1] = *(IntfPtr+2);
        FourByteUnion.ByteValue[0] = *(IntfPtr+3);

        return FourByteUnion.IntValue;
}
AdiInt32 get_long_swap( AdiUInt8 * IntfPtr )
{
  union
  {
          AdiInt32            IntValue;
          AdiUInt8   ByteValue[4];
  } FourByteUnion;

  FourByteUnion.ByteValue[3] = *IntfPtr;
  FourByteUnion.ByteValue[2] = *(IntfPtr+1);
  FourByteUnion.ByteValue[1] = *(IntfPtr+2);
  FourByteUnion.ByteValue[0] = *(IntfPtr+3);

  return FourByteUnion.IntValue;
}
AdiInt64 get_int64_swap( AdiUInt8 * IntfPtr )
{
	union
	{
		AdiInt64            IntValue;
		AdiUInt8   ByteValue[8];
	} EightByteUnion;

	EightByteUnion.ByteValue[7] = *IntfPtr;
	EightByteUnion.ByteValue[6] = *(IntfPtr+1);
	EightByteUnion.ByteValue[5] = *(IntfPtr+2);
	EightByteUnion.ByteValue[4] = *(IntfPtr+3);
	EightByteUnion.ByteValue[3] = *(IntfPtr+4);
	EightByteUnion.ByteValue[2] = *(IntfPtr+5);
	EightByteUnion.ByteValue[1] = *(IntfPtr+6);
	EightByteUnion.ByteValue[0] = *(IntfPtr+7);

	return EightByteUnion.IntValue;
}
void set_float_swap( AdiUInt8 * IntfPtr, AdiFloat32 value )
{
        union
        {
                AdiFloat32                 FloatValue;
                AdiUInt8   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.FloatValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[3];
        *(IntfPtr+1) = FourByteUnion.ByteValue[2];
        *(IntfPtr+2) = FourByteUnion.ByteValue[1];
        *(IntfPtr+3) = FourByteUnion.ByteValue[0];
}
void set_double_swap( AdiUInt8 * IntfPtr, AdiFloat64 value )
{
        union
        {
                AdiFloat64                 DoubleValue;
                AdiUInt8   ByteValue[8];
        } EightByteUnion;

        EightByteUnion.DoubleValue = value;

        *(IntfPtr)   = EightByteUnion.ByteValue[7];
        *(IntfPtr+1) = EightByteUnion.ByteValue[6];
        *(IntfPtr+2) = EightByteUnion.ByteValue[5];
        *(IntfPtr+3) = EightByteUnion.ByteValue[4];
        *(IntfPtr+4) = EightByteUnion.ByteValue[3];
        *(IntfPtr+5) = EightByteUnion.ByteValue[2];
        *(IntfPtr+6) = EightByteUnion.ByteValue[1];
        *(IntfPtr+7) = EightByteUnion.ByteValue[0];
}
void set_short_swap( AdiUInt8 * IntfPtr, AdiInt16 value )
{
        union
        {
                AdiInt16          IntValue;
                AdiUInt8   ByteValue[2];
        } TwoByteUnion;

        TwoByteUnion.IntValue = value;

        *(IntfPtr)   = TwoByteUnion.ByteValue[1];
        *(IntfPtr+1) = TwoByteUnion.ByteValue[0];
}
void set_int_swap( AdiUInt8 * IntfPtr, AdiInt value )
{
        union
        {
                AdiInt32                  IntValue;
                AdiUInt8   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.IntValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[3];
        *(IntfPtr+1) = FourByteUnion.ByteValue[2];
        *(IntfPtr+2) = FourByteUnion.ByteValue[1];
        *(IntfPtr+3) = FourByteUnion.ByteValue[0];
}

void set_long_swap( AdiUInt8 * IntfPtr, AdiInt32 value )
{
        union
        {
                AdiInt32                  IntValue;
                AdiUInt8   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.IntValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[3];
        *(IntfPtr+1) = FourByteUnion.ByteValue[2];
        *(IntfPtr+2) = FourByteUnion.ByteValue[1];
        *(IntfPtr+3) = FourByteUnion.ByteValue[0];
}
void set_int64_swap( AdiUInt8 * IntfPtr, AdiInt64 value )
{
	union
	{
		AdiInt64                  IntValue;
		AdiUInt8   ByteValue[8];
	} EightByteUnion;

	EightByteUnion.IntValue = value;

	*(IntfPtr)   = EightByteUnion.ByteValue[7];
	*(IntfPtr+1) = EightByteUnion.ByteValue[6];
	*(IntfPtr+2) = EightByteUnion.ByteValue[5];
	*(IntfPtr+3) = EightByteUnion.ByteValue[4];
	*(IntfPtr+4) = EightByteUnion.ByteValue[3];
	*(IntfPtr+5) = EightByteUnion.ByteValue[2];
	*(IntfPtr+6) = EightByteUnion.ByteValue[1];
	*(IntfPtr+7) = EightByteUnion.ByteValue[0];
}

void swap_buffer(AdiUInt8* dest,AdiInt size,AdiUInt8* source,AdiInt length)
{
	AdiInt i;
	if (size>=length)
	{
		for (i=0;i<length;i++)
		{
			dest[i]=source[length-i-1];
		}
	}
}

AdiUInt64 POWER_2[64]=
{
0x1,
0x2,
0x4,
0x8,
0x10,
0x20,
0x40,
0x80,
0x100,
0x200,
0x400,
0x800,
0x1000,
0x2000,
0x4000,
0x8000,
0x10000,
0x20000,
0x40000,
0x80000,
0x100000,
0x200000,
0x400000,
0x800000,
0x1000000,
0x2000000,
0x4000000,
0x8000000,
0x10000000,
0x20000000,
0x40000000,
0x80000000,
0x100000000,
0x200000000,
0x400000000,
0x800000000,
0x1000000000,
0x2000000000,
0x4000000000,
0x8000000000,
0x10000000000,
0x20000000000,
0x40000000000,
0x80000000000,
0x100000000000,
0x200000000000,
0x400000000000,
0x800000000000,
0x1000000000000,
0x2000000000000,
0x4000000000000,
0x8000000000000,
0x10000000000000,
0x20000000000000,
0x40000000000000,
0x80000000000000,
0x100000000000000,
0x200000000000000,
0x400000000000000,
0x800000000000000,
0x1000000000000000,
0x2000000000000000,
0x4000000000000000,
0x8000000000000000
};

AdiUInt64 power2(AdiInt exponent)
{
	return POWER_2[exponent];
}

void swap_bytes_uint16_array(uint16_t* arr, int arrLength)
{
    for (int i = 0; i < arrLength; ++i) {
        // Swap the bytes within the word
        uint16_t v = arr[i];
        arr[i] = (uint16_t)((v >> 8) | (v << 8));
    }
}
