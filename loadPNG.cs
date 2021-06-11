public static Texture2D LoadPng(Stream fileStream)
{
	Texture2D texture = null;

	if (fileStream != null)
	{
		using (var memoryStream = new MemoryStream())
		{
			fileStream.CopyTo(memoryStream);

			texture = new Texture2D(2, 2);
			texture.LoadImage(memoryStream.ToArray()); //This will auto-resize the texture dimensions.
		}
	}

	return texture;
}
