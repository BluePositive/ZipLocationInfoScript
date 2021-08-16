// Copyright 2021 blueposivecode@gmail.com All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZipLocationInfoScript
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            var path = Path.Combine(projectDirectory, "Files");

            Dictionary<string, (string city, string State)> cityStateDic =
            (await File.ReadAllLinesAsync($"{path}\\free-zipcode-database-Primary.csv"))
                .Select(line => line.Split(','))
                .Skip(1)
                .ToDictionary(line => line[0][1..^1], line => (line[2][1..^1], line[3][1..^1]));

            var geoCoordinates = await File.ReadAllLinesAsync($"{path}\\USZipCodeGeolocations2018.csv");

            IEnumerable<string> CreateFileInfo(IEnumerable<string> lines)
            {
                yield return "ZIP,LAT,LNG,City,State";
                foreach (var line in lines)
                {
                    var zip = line.Split(',')[0];
                    if (!cityStateDic.TryGetValue(zip, out var result))
                        yield return $"{line},,";
                    var (city, state) = result;

                    yield return $"{line},{city},{state}";
                }
            }

            var fileInfo = CreateFileInfo(geoCoordinates.Skip(1));

            await File.WriteAllLinesAsync($"{path}\\USZipCodeGeolocations2018WithCityAndState.csv", fileInfo);

        }
    }
}
