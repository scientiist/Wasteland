using System.Collections.Generic;
using System;

namespace Wasteland.Common
{
	using Dataset = Dictionary<string, string>;
	public enum Language
	{
		English,
		German,
		Spanish,

	}


	
	// String Directory for localization;
    public static class GameStrings
    {
		public static string CopyrightNotice => CurrentDataset["copyright_notice"];



		public static Language CurrentLanguage {get;set;} = Language.English;
		

		// TODO: Load this dataset from localization files

		public static Dataset CurrentDataset => Database[CurrentLanguage];

		// limited font, so no Copyright symbol :(
		static Dataset EnglishStrings = new Dataset
		{
			["copyright_notice"] = "Copyright 2019-2021 Conarium Software",
		};
		static Dataset GermanStrings = new Dataset
		{
			["copyright_notice"] = "Copyright 2019-2021 Conarium Software",
		};
		static Dataset SpanishStrings = new Dataset
		{
			["copyright_notice"] = "Copyright 2019-2021 Conarium Software",
		};

		public static Dictionary<Language, Dataset> Database = new Dictionary<Language, Dataset>
		{
			[Language.English] = EnglishStrings,
			[Language.Spanish] = SpanishStrings,
			[Language.German] = GermanStrings,
		
		};
    }
}
