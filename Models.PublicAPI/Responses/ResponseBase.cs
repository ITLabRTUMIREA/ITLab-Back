﻿using System;
namespace Models.PublicAPI.Responses
{
    public class ResponseBase
    {
        public ResponseStatusCode StatusCode { get; }

        public ResponseBase(ResponseStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
}