
#ifndef CONVLIB_H
#define CONVLIB_H

// convlib.h
#include <stdlib.h>
#include <string.h>
#include "typelib.h"

#define __int64  long long
#define check_if_float(X ,Y) 1
#define check_if_double(X ,Y) 1

#ifdef __cplusplus
extern "C" {
#endif


static AdiUInt16  get_ushort_align( AdiUInt8 * IntfPtr ){return *(AdiUInt16*)IntfPtr;}
static AdiUInt16  get_ushort_swap( AdiUInt8 * IntfPtr ) {return _byteswap_ushort(*(unsigned short*)IntfPtr);}

static AdiInt16   get_short_align( AdiUInt8 * IntfPtr ) {return *(AdiInt16*)IntfPtr;}
static AdiInt16   get_short_swap( AdiUInt8 * IntfPtr )  {return _byteswap_ushort(*(unsigned short*)IntfPtr);}

static AdiUInt32  get_uint_align( AdiUInt8 * IntfPtr )  {return *(AdiUInt32*)IntfPtr; }
static AdiUInt32  get_uint_swap( AdiUInt8 * IntfPtr )   {return _byteswap_ulong(*(unsigned long*)IntfPtr);}

static AdiInt     get_int_align( AdiUInt8 * IntfPtr )   {return *(AdiInt*)IntfPtr;}
       AdiInt	  get_int_swap( AdiUInt8 * IntfPtr );

static AdiUInt32  get_ulong_align( AdiUInt8 * IntfPtr ) {return *(AdiUInt32*)IntfPtr;}
static AdiUInt32  get_ulong_swap( AdiUInt8 * IntfPtr )  {return _byteswap_ulong(*(unsigned long*)IntfPtr);}

static AdiInt32   get_long_align( AdiUInt8 * IntfPtr )  {return *(AdiInt32*)IntfPtr;}
       AdiInt32   get_long_swap( AdiUInt8 * IntfPtr );

static AdiUInt64  get_uint64_align( AdiUInt8 * IntfPtr ){return *(AdiUInt64*)IntfPtr;}
static AdiUInt64  get_uint64_swap( AdiUInt8 * IntfPtr ) {return _byteswap_uint64(*(unsigned __int64*)IntfPtr);}

static AdiInt64   get_int64_align( AdiUInt8 * IntfPtr ) {return *(AdiInt64*)IntfPtr;}
       AdiInt64   get_int64_swap( AdiUInt8 * IntfPtr );

static AdiFloat32 get_float_align(AdiUInt8* IntfPtr)    {return *(AdiFloat32*)IntfPtr; }
       AdiFloat32 get_float_swap(AdiUInt8* IntfPtr);

static AdiFloat64 get_double_align(AdiUInt8* IntfPtr)   {return *(AdiFloat64*)IntfPtr; }
       AdiFloat64 get_double_swap(AdiUInt8* IntfPtr);
//AdiInt check_if_real( AdiFloat32 value );

AdiUInt64 power2(AdiInt exponent);

AdiInt64 convert_from_sign_magnitude(AdiUInt64 value, AdiInt maskSize);
AdiUInt64 convert_to_sign_magnitude(AdiInt64 value, AdiInt maskSize);
AdiInt64 convert_from_ones_complement(AdiUInt64 value, AdiInt maskSize);
AdiUInt64 convert_to_ones_complement(AdiInt64 value, AdiInt maskSize);
AdiInt64 convert_from_twos_complement(AdiUInt64 value, AdiInt maskSize);
AdiUInt64 convert_to_twos_complement(AdiInt64 value, AdiInt maskSize);
//void set_AdiFloat32_align( AdiUInt8 * const IntfPtr, AdiFloat32 value );
//void set_AdiFloat32_swap( AdiUInt8 * const IntfPtr, AdiFloat32 value );
//
//void set_AdiFloat64_align( AdiUInt8 * const IntfPtr, AdiFloat64 value );
//void set_AdiFloat64_swap( AdiUInt8 * const IntfPtr, AdiFloat64 value );


static void set_ushort_align(AdiUInt8* const IntfPtr, AdiUInt16 value) { *(AdiUInt16*)IntfPtr = value;}
static void set_ushort_swap(AdiUInt8* const IntfPtr, AdiUInt16 value)  { *(AdiUInt16*)IntfPtr = _byteswap_ushort(value);}

static void set_short_align(AdiUInt8 * const IntfPtr, AdiInt16 value ) { *(AdiInt16*)IntfPtr = value; }
       void set_short_swap(AdiUInt8 * const IntfPtr, AdiInt16 value );

static void set_uint_align(AdiUInt8 * const IntfPtr, AdiUInt32 value ) { *(AdiUInt32*)IntfPtr = value; }
static void set_uint_swap(AdiUInt8 * const IntfPtr, AdiUInt32 value ) { *(AdiUInt32*)IntfPtr = _byteswap_ulong(value); }

static void set_int_align(AdiUInt8 * const IntfPtr, AdiInt value ) { *(AdiInt*)IntfPtr = value; }
       void set_int_swap(AdiUInt8 * const IntfPtr, AdiInt value );

static void set_ulong_align(AdiUInt8 * const IntfPtr, AdiUInt32 value ) { *(AdiUInt32*)IntfPtr = value; }
static void set_ulong_swap( AdiUInt8 * IntfPtr, AdiUInt32 value ) { *(AdiUInt32*)IntfPtr = _byteswap_ulong(value); }

static void set_long_align(AdiUInt8 * const IntfPtr, AdiInt32 value ) { *(AdiInt32*)IntfPtr = value; }
        void set_long_swap(AdiUInt8 * const IntfPtr, AdiInt32 value );

static void set_uint64_align( AdiUInt8 * IntfPtr, AdiUInt64 value ) { *(AdiUInt64*)IntfPtr = value; }
static void set_uint64_swap( AdiUInt8 * IntfPtr, AdiUInt64 value ) { *(AdiInt64*)IntfPtr = _byteswap_uint64(value); }

static void set_int64_align( AdiUInt8 * IntfPtr, AdiInt64 value ) { *(AdiInt64*)IntfPtr = value; }
       void set_int64_swap( AdiUInt8 * IntfPtr, AdiInt64 value );

static void set_float_align(AdiUInt8 * IntfPtr, AdiFloat32 value) { *(AdiFloat32*)IntfPtr = value; }
       void set_float_swap( AdiUInt8 * IntfPtr, AdiFloat32 value );

static void set_double_align(AdiUInt8 * IntfPtr, AdiFloat64 value) { *(AdiFloat64*)IntfPtr = value; }
       void set_double_swap( AdiUInt8 * IntfPtr, AdiFloat64 value );

static void align_buffer(AdiUInt8* dest, AdiInt size, AdiUInt8* source, AdiInt length) { memcpy(dest, source, length); }
void swap_buffer(AdiUInt8* dest,AdiInt size,AdiUInt8* source,AdiInt length);

//AdiInt check_if_float(AdiUInt8* buffer, AdiInt length);
//AdiInt check_if_double(AdiUInt8* buffer, AdiInt length);

/* define CUSTOM_REAL_CHECKS and declare your custom functions in application code */
extern AdiInt check_if_AdiFloat32(AdiUInt8* buffer,AdiInt length);
extern AdiInt check_if_AdiFloat64(AdiUInt8* buffer,AdiInt length);

void swap_bytes_uint16_array(uint16_t* arr, int arrLength);
#ifdef __cplusplus
}
#endif

#endif



