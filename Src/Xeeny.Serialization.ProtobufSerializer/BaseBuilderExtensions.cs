﻿using Xeeny.Api.Base;
using Xeeny.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xeeny
{
    public static class BaseBuilderExtensions
    {
        public static TBuilder WithProtobufSerializer<TBuilder>(this TBuilder builder)
            where TBuilder : BaseBuilder
        {
            builder.Serializer = new ProtobufSerializer();
            return builder;
        }
    }
}
