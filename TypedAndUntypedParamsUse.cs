public static void TypedParameters<T>(params T[] incoming)
{
	foreach (T param in incoming)
	{
		// Do your thing
	}
}

public static void AnyTypeParameters(params object[] incoming)
{
	foreach (object param in incoming)
	{
		if (param is int)
		{
			// Its an int
			// Do your thing
		}
		else if (param is float)
		{
			// Its a float
			// Do your thing
		}
		else if (param is string)
		{
			// Its a string
			// Do your thing
		}
		else if (param is List<int>)
		{
			// Its a list of ints
			foreach(int subParam in (List<int>)param)
			{
				// Do your thing
			}
		}
		else if (param is List<float>)
		{
			// Its a list of floats
			foreach (float subParam in (List<float>)param)
			{
				// Do your thing
			}
		}
		else if (param is List<string>)
		{
			// Its a list of strings
			foreach (string subParam in (List<string>)param)
			{
				// Do your thing
			}
		}
		else if (param is Vector3)
		{
			// Its a Vector3
			// Do your thing
		}
	}
}
