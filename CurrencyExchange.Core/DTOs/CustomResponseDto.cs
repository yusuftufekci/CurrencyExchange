﻿using System.Text.Json.Serialization;

namespace CurrencyExchange.Core.DTOs
{
    public class CustomResponseDto<T>
    {
        public T Data { get; set; }
        [JsonIgnore] public int StatusCode { get; set; }

        public List<String> Errors { get; set; }


        public static CustomResponseDto<T> Success(int statusCode, T data)
        {
            return new CustomResponseDto<T> { StatusCode = statusCode, Data = data };
        }

        public static CustomResponseDto<T> Success(T data)
        {
            return new CustomResponseDto<T> { Data = data };
        }
        public static CustomResponseDto<T> Success()
        {
            return new CustomResponseDto<T> { };
        }
        public static CustomResponseDto<T> Fail(int statusCode, List<string> errors)
        {
            return new CustomResponseDto<T> { Errors = errors, StatusCode = statusCode };
        }

        public static CustomResponseDto<T> Fail(int statusCode, string error)
        {
            return new CustomResponseDto<T> { Errors = new List<string> { error }, StatusCode = statusCode };
        }
    }
}