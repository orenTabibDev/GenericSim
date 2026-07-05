/************************************************************************
 * File:        convlib.c                                               
 * Author:      Adi Mainly SW                                              
 *                                                                      
 * Description: Big Endian <-> Little Endian Converts & data align    
 *                                                                      
 * Procedures:                                                                                              
 *              many...                                                                                     
 *                                                                      
 * Last Updated:  
 *
 *			30-09-09 E.H. - cosmetics                                                    
 *			26-05-08 N.G. - custom real/double check, align/swap buffer fumctions added                                                    
 *                                                                      
 ************************************************************************/


#include <stdio.h>
#include <float.h> /* include for Windows-based system */
#ifndef __INTIME__
	#include <memory.h>
#else
	#include <string.h>
	#include <math.h>
	#define _finite(x)	isfinite(x)	
#endif
#include "convlib.h"

#ifndef CUSTOM_REAL_CHECKS

int check_if_float(unsigned char* /*buffer*/,int /*length*/)
{
	return 1;
}

int check_if_double(unsigned char* /*buffer*/,int /*length*/)
{
	return 1;
}

#endif

/* All of these functions written for the SPARC & Intel Integers  */
/* MSB FIRST                                                                                    */
/* LSB LAST                                                                                      */

int check_if_real( float value )
{
   return _finite(value);
}

float get_float_align( unsigned char * IntfPtr )
{
        union
        {
                float                 FloatValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.ByteValue[0] = *IntfPtr;
        FourByteUnion.ByteValue[1] = *(IntfPtr+1);
        FourByteUnion.ByteValue[2] = *(IntfPtr+2);
        FourByteUnion.ByteValue[3] = *(IntfPtr+3);

        return FourByteUnion.FloatValue;

}


float get_float_swap( unsigned char * IntfPtr )
{

        union
        {
                float                 FloatValue;
                unsigned char   ByteValue[4];
                unsigned long  LongValue;
        } FourByteUnion;

        FourByteUnion.ByteValue[3] = *IntfPtr;
        FourByteUnion.ByteValue[2] = *(IntfPtr+1);
        FourByteUnion.ByteValue[1] = *(IntfPtr+2);
        FourByteUnion.ByteValue[0] = *(IntfPtr+3);

        if(FourByteUnion.LongValue)
                return FourByteUnion.FloatValue;
        else    return 0.0;

}

double get_double_align( unsigned char * IntfPtr )
{
        union
        {
                double                 DoubleValue;
                unsigned char   ByteValue[8];
        } EightByteUnion;

        EightByteUnion.ByteValue[0] = *IntfPtr;
        EightByteUnion.ByteValue[1] = *(IntfPtr+1);
        EightByteUnion.ByteValue[2] = *(IntfPtr+2);
        EightByteUnion.ByteValue[3] = *(IntfPtr+3);
        EightByteUnion.ByteValue[4] = *(IntfPtr+4);
        EightByteUnion.ByteValue[5] = *(IntfPtr+5);
        EightByteUnion.ByteValue[6] = *(IntfPtr+6);
        EightByteUnion.ByteValue[7] = *(IntfPtr+7);

        return EightByteUnion.DoubleValue;

}


double get_double_swap( unsigned char * IntfPtr )
{

        union
        {
                double                 DoubleValue;
                unsigned char   ByteValue[8];
                unsigned long  LongValue;
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

unsigned short int get_ushort_align( unsigned char * IntfPtr )
{
        union
        {
                unsigned short int      IntValue;
                unsigned char         ByteValue[2];
        } TwoByteUnion;

        TwoByteUnion.ByteValue[0] = *IntfPtr;
        TwoByteUnion.ByteValue[1] = *(IntfPtr+1);

        return TwoByteUnion.IntValue;
}

unsigned short int get_ushort_swap( unsigned char * IntfPtr )
{
        union
        {
                unsigned short int      IntValue;
                unsigned char         ByteValue[2];
        } TwoByteUnion;

        TwoByteUnion.ByteValue[1] = *IntfPtr;
        TwoByteUnion.ByteValue[0] = *(IntfPtr+1);

        return TwoByteUnion.IntValue;
}

short int get_short_align( unsigned char * IntfPtr )
{
        union
        {
                short int          IntValue;
                unsigned char   ByteValue[2];
        } TwoByteUnion;

        TwoByteUnion.ByteValue[0] = *IntfPtr;
        TwoByteUnion.ByteValue[1] = *(IntfPtr+1);

        return TwoByteUnion.IntValue;
}

short int get_short_swap( unsigned char * IntfPtr )
{
        union
        {
                short int          IntValue;
                unsigned char   ByteValue[2];
        } TwoByteUnion;

        TwoByteUnion.ByteValue[1] = *IntfPtr;
        TwoByteUnion.ByteValue[0] = *(IntfPtr+1);

        return TwoByteUnion.IntValue;
}

unsigned int get_uint_align( unsigned char * IntfPtr )
{
        union
        {
                unsigned int    IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.ByteValue[0] = *IntfPtr;
        FourByteUnion.ByteValue[1] = *(IntfPtr+1);
        FourByteUnion.ByteValue[2] = *(IntfPtr+2);
        FourByteUnion.ByteValue[3] = *(IntfPtr+3);

        return FourByteUnion.IntValue;
}

unsigned int get_uint_swap( unsigned char * IntfPtr )
{
        union
        {
                unsigned int    IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.ByteValue[3] = *IntfPtr;
        FourByteUnion.ByteValue[2] = *(IntfPtr+1);
        FourByteUnion.ByteValue[1] = *(IntfPtr+2);
        FourByteUnion.ByteValue[0] = *(IntfPtr+3);

        return FourByteUnion.IntValue;
}

int get_int_align( unsigned char * IntfPtr )
{
        union
        {
                int                   IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.ByteValue[0] = *IntfPtr;
        FourByteUnion.ByteValue[1] = *(IntfPtr+1);
        FourByteUnion.ByteValue[2] = *(IntfPtr+2);
        FourByteUnion.ByteValue[3] = *(IntfPtr+3);

        return FourByteUnion.IntValue;
}

int get_int_swap( unsigned char * IntfPtr )
{
        union
        {
                int                   IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.ByteValue[3] = *IntfPtr;
        FourByteUnion.ByteValue[2] = *(IntfPtr+1);
        FourByteUnion.ByteValue[1] = *(IntfPtr+2);
        FourByteUnion.ByteValue[0] = *(IntfPtr+3);

        return FourByteUnion.IntValue;
}

unsigned long get_ulong_align( unsigned char * IntfPtr )
{
        union
        {
                unsigned long    IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.ByteValue[0] = *IntfPtr;
        FourByteUnion.ByteValue[1] = *(IntfPtr+1);
        FourByteUnion.ByteValue[2] = *(IntfPtr+2);
        FourByteUnion.ByteValue[3] = *(IntfPtr+3);

        return FourByteUnion.IntValue;
}

unsigned long get_ulong_swap( unsigned char * IntfPtr )
{
  union
  {
          unsigned long    IntValue;
          unsigned char   ByteValue[4];
  } FourByteUnion;

  FourByteUnion.ByteValue[3] = *IntfPtr;
  FourByteUnion.ByteValue[2] = *(IntfPtr+1);
  FourByteUnion.ByteValue[1] = *(IntfPtr+2);
  FourByteUnion.ByteValue[0] = *(IntfPtr+3);

  return FourByteUnion.IntValue;
}

long get_long_align( unsigned char * IntfPtr )
{
  union
  {
          long						IntValue;
          unsigned char   ByteValue[4];
  } FourByteUnion;

  FourByteUnion.ByteValue[0] = *IntfPtr;
  FourByteUnion.ByteValue[1] = *(IntfPtr+1);
  FourByteUnion.ByteValue[2] = *(IntfPtr+2);
  FourByteUnion.ByteValue[3] = *(IntfPtr+3);

  return FourByteUnion.IntValue;
}

long get_long_swap( unsigned char * IntfPtr )
{
  union
  {
          long            IntValue;
          unsigned char   ByteValue[4];
  } FourByteUnion;

  FourByteUnion.ByteValue[3] = *IntfPtr;
  FourByteUnion.ByteValue[2] = *(IntfPtr+1);
  FourByteUnion.ByteValue[1] = *(IntfPtr+2);
  FourByteUnion.ByteValue[0] = *(IntfPtr+3);

  return FourByteUnion.IntValue;
}

unsigned __int64 get_uint64_align( unsigned char * IntfPtr )
{
	union
	{
		unsigned __int64            IntValue;
		unsigned char   ByteValue[8];
	} EightByteUnion;

	EightByteUnion.ByteValue[0] = *IntfPtr;
	EightByteUnion.ByteValue[1] = *(IntfPtr+1);
	EightByteUnion.ByteValue[2] = *(IntfPtr+2);
	EightByteUnion.ByteValue[3] = *(IntfPtr+3);
	EightByteUnion.ByteValue[4] = *(IntfPtr+4);
	EightByteUnion.ByteValue[5] = *(IntfPtr+5);
	EightByteUnion.ByteValue[6] = *(IntfPtr+6);
	EightByteUnion.ByteValue[7] = *(IntfPtr+7);

	return EightByteUnion.IntValue;
}

unsigned __int64 get_uint64_swap( unsigned char * IntfPtr )
{
	union
	{
		unsigned __int64            IntValue;
		unsigned char   ByteValue[8];
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

__int64 get_int64_align( unsigned char * IntfPtr )
{
	union
	{
		__int64            IntValue;
		unsigned char   ByteValue[8];
	} EightByteUnion;

	EightByteUnion.ByteValue[0] = *IntfPtr;
	EightByteUnion.ByteValue[1] = *(IntfPtr+1);
	EightByteUnion.ByteValue[2] = *(IntfPtr+2);
	EightByteUnion.ByteValue[3] = *(IntfPtr+3);
	EightByteUnion.ByteValue[4] = *(IntfPtr+4);
	EightByteUnion.ByteValue[5] = *(IntfPtr+5);
	EightByteUnion.ByteValue[6] = *(IntfPtr+6);
	EightByteUnion.ByteValue[7] = *(IntfPtr+7);

	return EightByteUnion.IntValue;
}

__int64 get_int64_swap( unsigned char * IntfPtr )
{
	union
	{
		__int64            IntValue;
		unsigned char   ByteValue[8];
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


/**************************************************************************
*
* SET FUNCTIONS
*
**************************************************************************/

void set_float_align( unsigned char * IntfPtr, float value )
{
        union
        {
                float                 FloatValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.FloatValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[0];
        *(IntfPtr+1) = FourByteUnion.ByteValue[1];
        *(IntfPtr+2) = FourByteUnion.ByteValue[2];
        *(IntfPtr+3) = FourByteUnion.ByteValue[3];
}

void set_float_swap( unsigned char * IntfPtr, float value )
{
        union
        {
                float                 FloatValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.FloatValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[3];
        *(IntfPtr+1) = FourByteUnion.ByteValue[2];
        *(IntfPtr+2) = FourByteUnion.ByteValue[1];
        *(IntfPtr+3) = FourByteUnion.ByteValue[0];
}

void set_double_align( unsigned char * IntfPtr, double value )
{
        union
        {
                double                 DoubleValue;
                unsigned char   ByteValue[8];
        } EightByteUnion;

        EightByteUnion.DoubleValue = value;

        *(IntfPtr)   = EightByteUnion.ByteValue[0];
        *(IntfPtr+1) = EightByteUnion.ByteValue[1];
        *(IntfPtr+2) = EightByteUnion.ByteValue[2];
        *(IntfPtr+3) = EightByteUnion.ByteValue[3];
        *(IntfPtr+4) = EightByteUnion.ByteValue[4];
        *(IntfPtr+5) = EightByteUnion.ByteValue[5];
        *(IntfPtr+6) = EightByteUnion.ByteValue[6];
        *(IntfPtr+7) = EightByteUnion.ByteValue[7];
}

void set_double_swap( unsigned char * IntfPtr, double value )
{
        union
        {
                double                 DoubleValue;
                unsigned char   ByteValue[8];
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


void set_ushort_align( unsigned char * IntfPtr, unsigned short int value )
{
        union
        {
                unsigned short int      IntValue;
                unsigned char              ByteValue[2];
        } TwoByteUnion;

        TwoByteUnion.IntValue = value;

        *(IntfPtr)   = TwoByteUnion.ByteValue[0];
        *(IntfPtr+1) = TwoByteUnion.ByteValue[1];
}

void set_ushort_swap( unsigned char * IntfPtr, unsigned short int value )
{
        union
        {
                unsigned short int      IntValue;
                unsigned char              ByteValue[2];
        } TwoByteUnion;

        TwoByteUnion.IntValue = value;

        *(IntfPtr)   = TwoByteUnion.ByteValue[1];
        *(IntfPtr+1) = TwoByteUnion.ByteValue[0];
}

void set_short_align( unsigned char * IntfPtr, short int value )
{
        union
        {
                short int          IntValue;
                unsigned char   ByteValue[2];
        } TwoByteUnion;

        TwoByteUnion.IntValue = value;

        *(IntfPtr)   = TwoByteUnion.ByteValue[0];
        *(IntfPtr+1) = TwoByteUnion.ByteValue[1];
}

void set_short_swap( unsigned char * IntfPtr, short int value )
{
        union
        {
                short int          IntValue;
                unsigned char   ByteValue[2];
        } TwoByteUnion;

        TwoByteUnion.IntValue = value;

        *(IntfPtr)   = TwoByteUnion.ByteValue[1];
        *(IntfPtr+1) = TwoByteUnion.ByteValue[0];
}

void set_uint_align( unsigned char * IntfPtr, unsigned int value )
{
        union
        {
                unsigned int    IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.IntValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[0];
        *(IntfPtr+1) = FourByteUnion.ByteValue[1];
        *(IntfPtr+2) = FourByteUnion.ByteValue[2];
        *(IntfPtr+3) = FourByteUnion.ByteValue[3];
}

void set_uint_swap( unsigned char * IntfPtr, unsigned int value )
{
        union
        {
                unsigned int    IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.IntValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[3];
        *(IntfPtr+1) = FourByteUnion.ByteValue[2];
        *(IntfPtr+2) = FourByteUnion.ByteValue[1];
        *(IntfPtr+3) = FourByteUnion.ByteValue[0];
}

void set_int_align( unsigned char * IntfPtr, int value )
{
        union
        {
                int                   IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.IntValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[0];
        *(IntfPtr+1) = FourByteUnion.ByteValue[1];
        *(IntfPtr+2) = FourByteUnion.ByteValue[2];
        *(IntfPtr+3) = FourByteUnion.ByteValue[3];
}

void set_int_swap( unsigned char * IntfPtr, int value )
{
        union
        {
                long                  IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.IntValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[3];
        *(IntfPtr+1) = FourByteUnion.ByteValue[2];
        *(IntfPtr+2) = FourByteUnion.ByteValue[1];
        *(IntfPtr+3) = FourByteUnion.ByteValue[0];
}

void set_ulong_align( unsigned char * IntfPtr, unsigned long value )
{
        union
        {
                unsigned long   IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.IntValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[0];
        *(IntfPtr+1) = FourByteUnion.ByteValue[1];
        *(IntfPtr+2) = FourByteUnion.ByteValue[2];
        *(IntfPtr+3) = FourByteUnion.ByteValue[3];
}

void set_ulong_swap( unsigned char * IntfPtr, unsigned long value )
{
        union
        {
                unsigned long   IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.IntValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[3];
        *(IntfPtr+1) = FourByteUnion.ByteValue[2];
        *(IntfPtr+2) = FourByteUnion.ByteValue[1];
        *(IntfPtr+3) = FourByteUnion.ByteValue[0];
}

void set_long_align( unsigned char * IntfPtr, long value )
{
        union
        {
                long            IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.IntValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[0];
        *(IntfPtr+1) = FourByteUnion.ByteValue[1];
        *(IntfPtr+2) = FourByteUnion.ByteValue[2];
        *(IntfPtr+3) = FourByteUnion.ByteValue[3];
}

void set_long_swap( unsigned char * IntfPtr, long value )
{
        union
        {
                long                  IntValue;
                unsigned char   ByteValue[4];
        } FourByteUnion;

        FourByteUnion.IntValue = value;

        *(IntfPtr)   = FourByteUnion.ByteValue[3];
        *(IntfPtr+1) = FourByteUnion.ByteValue[2];
        *(IntfPtr+2) = FourByteUnion.ByteValue[1];
        *(IntfPtr+3) = FourByteUnion.ByteValue[0];
}

void set_uint64_align( unsigned char * IntfPtr, unsigned __int64 value )
{
	union
	{
		unsigned __int64                  IntValue;
		unsigned char   ByteValue[8];
	} EightByteUnion;

	EightByteUnion.IntValue = value;

	*(IntfPtr)   = EightByteUnion.ByteValue[0];
	*(IntfPtr+1) = EightByteUnion.ByteValue[1];
	*(IntfPtr+2) = EightByteUnion.ByteValue[2];
	*(IntfPtr+3) = EightByteUnion.ByteValue[3];
	*(IntfPtr+4) = EightByteUnion.ByteValue[4];
	*(IntfPtr+5) = EightByteUnion.ByteValue[5];
	*(IntfPtr+6) = EightByteUnion.ByteValue[6];
	*(IntfPtr+7) = EightByteUnion.ByteValue[7];
}

void set_uint64_swap( unsigned char * IntfPtr, unsigned __int64 value )
{
	union
	{
		unsigned __int64                  IntValue;
		unsigned char   ByteValue[8];
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

void set_int64_align( unsigned char * IntfPtr, __int64 value )
{
	union
	{
		__int64                  IntValue;
		unsigned char   ByteValue[8];
	} EightByteUnion;

	EightByteUnion.IntValue = value;

	*(IntfPtr)   = EightByteUnion.ByteValue[0];
	*(IntfPtr+1) = EightByteUnion.ByteValue[1];
	*(IntfPtr+2) = EightByteUnion.ByteValue[2];
	*(IntfPtr+3) = EightByteUnion.ByteValue[3];
	*(IntfPtr+4) = EightByteUnion.ByteValue[4];
	*(IntfPtr+5) = EightByteUnion.ByteValue[5];
	*(IntfPtr+6) = EightByteUnion.ByteValue[6];
	*(IntfPtr+7) = EightByteUnion.ByteValue[7];
}

void set_int64_swap( unsigned char * IntfPtr, __int64 value )
{
	union
	{
		__int64                  IntValue;
		unsigned char   ByteValue[8];
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

void align_buffer(unsigned char* dest,int /*size*/,unsigned char* source,int length)
{
/*#ifdef VS2005
	memcpy_s(dest,size,source,length);
else*/
	memcpy(dest, source, length);
}

void swap_buffer(unsigned char* dest,int size,unsigned char* source,int length)
{
	int i;
	if (size>=length)
	{
		for (i=0;i<length;i++)
		{
			dest[i]=source[length-i-1];
		}
	}
}



