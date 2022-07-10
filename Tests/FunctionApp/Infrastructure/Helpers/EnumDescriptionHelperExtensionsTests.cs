﻿using System.ComponentModel;
using Firepuma.EmailAndPush.FunctionApp.Infrastructure.Helpers;
using Firepuma.EmailAndPush.FunctionApp.Models.ValueObjects;
using Xunit;

namespace Tests.FunctionApp.Infrastructure.Helpers;

public class EnumDescriptionHelperExtensionsTests
{
    [Fact]
    public void GetEnumDescriptionOrNull_PushMessageUrgency_high_should_be_high()
    {
        // Arrange
        var value = PushMessageUrgency.High;

        // Act
        var description = value.GetEnumDescriptionOrNull();

        // Assert
        Assert.Equal("high", description);
    }
    
    [Fact]
    public void GetEnumDescriptionOrNull_AnotherTestEnum_RandomValue123_should_be_kebab_case()
    {
        // Arrange
        var value = AnotherTestEnum.RandomValue123;

        // Act
        var description = value.GetEnumDescriptionOrNull();

        // Assert
        Assert.Equal("random-value-123", description);
    }

    private enum AnotherTestEnum
    {
        [Description("random-value-123")]
        RandomValue123,
    }
}