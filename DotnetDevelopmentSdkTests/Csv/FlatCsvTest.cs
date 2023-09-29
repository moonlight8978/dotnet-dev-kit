// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Services.Csv;
using FluentAssertions;
using NUnit.Framework;

namespace DotnetDevelopmentSdkTests.Csv;

public class FlatCsvTest
{
    private enum BodyType
    {
        Arm, Leg, Torso, Core
    }

    private enum Element
    {
        Air, Fire, Water, Thunder
    }

    private enum MechaRarity
    {
        Legendary, Epic
    }

    private class MechaInfo
    {
        public int Id { get; set; }
        public MechaRarity Rarity { get; set; }
    }

    private class BodyPartInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private class CsvRow
    {
        public int MechaId { get; set; }
        public Dictionary<BodyType, Element> BodyPartTypeToElement { get; set; } = new();
        public string MechaName { get; set; } = null!;
        public MechaRarity MechaRarity { get; set; }
        public MechaInfo MechaInfo { get; set; }
        public Dictionary<BodyType, BodyPartInfo> BodyPartTypeToInfo { get; set; } = new();
    }

    [Test]
    public static async Task ExportDataCorrectly()
    {
        var csvTypeConverterProvider = new CsvTypeConverterProvider();
        var flatCsvExporter = new FlatCsvExporter(csvTypeConverterProvider);

        var rows = new List<CsvRow>
        {
            new()
            {
                MechaId = 1,
                MechaName = "Mecha 1",
                MechaRarity = MechaRarity.Epic,
                BodyPartTypeToElement =
                    new Dictionary<BodyType, Element> {{BodyType.Arm, Element.Air}, {BodyType.Core, Element.Fire}},
                MechaInfo = new MechaInfo {Id = 1, Rarity = MechaRarity.Epic},
                BodyPartTypeToInfo = new Dictionary<BodyType, BodyPartInfo>
                {
                    {BodyType.Arm, new BodyPartInfo {Id = 1, Name = "Arm"}},
                    {BodyType.Core, new BodyPartInfo {Id = 2, Name = "Core"}}
                }
            },
            new()
            {
                MechaId = 2,
                MechaName = "Mecha 2",
                MechaRarity = MechaRarity.Legendary,
                BodyPartTypeToElement =
                    new Dictionary<BodyType, Element>
                    {
                        {BodyType.Leg, Element.Thunder}, {BodyType.Torso, Element.Water}
                    },
                MechaInfo = new MechaInfo {Id = 2, Rarity = MechaRarity.Legendary},
                BodyPartTypeToInfo = new Dictionary<BodyType, BodyPartInfo>
                {
                    {BodyType.Leg, new BodyPartInfo {Id = 3, Name = "Leg"}},
                    {BodyType.Torso, new BodyPartInfo {Id = 4, Name = "Torso"}}
                }
            }
        };

        await flatCsvExporter.Export(rows, "test.csv");
    }

    [Test]
    public async Task ImportDataCorrectly()
    {
        var csvTypeConverterProvider = new CsvTypeConverterProvider();
        var flatCsvImporter = new FlatCsvImporter(csvTypeConverterProvider);
        var results = await flatCsvImporter.ImportAsync<CsvRow>("test.csv");
        results.Should().HaveCount(2);
    }
}
