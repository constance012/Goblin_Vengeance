﻿using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace CSTGames.DataPersistence
{
	public class SaveFileHandler<TData>
	{
		public string Directory { get; set; } = "";
		public string SubFolders { get; set; } = "";
		public string FileName { get; set; } = "";

		public bool UseEncryption { get; set; }

		private const string ENCRYPTION_CODE = "Sacrosanct";

		public SaveFileHandler(string directory, string subFolders, string fileName, bool useEncryption)
		{
			this.Directory = directory;
			this.SubFolders = subFolders;
			this.FileName = fileName;

			this.UseEncryption = useEncryption;
		}

		/// <summary>
		/// Load the data from a save slot using Newtonsoft's JSON.Net.
		/// </summary>
		/// <param name="saveSlotID"> The ID of the save slot to load data from. </param>
		/// <returns>The loaded Data</returns>
		public TData LoadDataFromFile(string saveSlotID)
		{
			if (saveSlotID == null)
				return default;

			string fullPath = Path.Combine(Directory, saveSlotID, SubFolders, FileName);

			TData loadedData = default;

			if (File.Exists(fullPath))
			{
				try
				{
					string serializedData = "";

					// Read the serialized data.
					using (FileStream file = new FileStream(fullPath, FileMode.Open))
					{
						using (StreamReader reader = new StreamReader(file))
						{
							serializedData = reader.ReadToEnd();
						}
					}

					// Decrypt the data (Optional).
					if (UseEncryption)
						serializedData = EncryptOrDecrypt(serializedData);

					// Deserialize that data from a json formatted string back into object's data.
					loadedData = JsonConvert.DeserializeObject<TData>(serializedData);
				}
				catch (Exception ex)
				{
					Debug.LogError($"Error occured when trying to load data from file.\n" +
							   $"At full path: {fullPath}.\n" +
							   $"Reason: {ex.Message}.");
				}
			}

			return loadedData;
		}

		/// <summary>
		/// Load the data from a standalone file using Newtonsoft's JSON.Net.
		/// </summary>
		/// <returns>The loaded Data</returns>
		public TData LoadDataFromFile()
		{
			string fullPath = Path.Combine(Directory, SubFolders, FileName);
			TData loadedData = default;

			if (File.Exists(fullPath))
			{
				try
				{
					string serializedData = "";

					// Read the serialized data.
					using (FileStream file = new FileStream(fullPath, FileMode.Open))
					{
						using (StreamReader reader = new StreamReader(file))
						{
							serializedData = reader.ReadToEnd();
						}
					}

					// Decrypt the data (Optional).
					if (UseEncryption)
						serializedData = EncryptOrDecrypt(serializedData);

					// Deserialize data.
					loadedData = JsonConvert.DeserializeObject<TData>(serializedData);
				}
				catch (Exception ex)
				{
					Debug.LogError($"Error occured when trying to load json from file.\n" +
							   $"At full path: {fullPath}.\n" +
							   $"Reason: {ex.Message}.");
				}
			}

			return loadedData;
		}

		/// <summary>
		/// Save the data of a save slot using Newtonsoft's JSON.Net.
		/// </summary>
		/// <param name="dataToSave"> The data object to save. </param>
		/// <param name="saveSlotID"> The ID of the save slot to load data from. </param>
		public void SaveDataToFile(TData dataToSave, string saveSlotID)
		{
			if (saveSlotID == null)
				return;

			string fullPath = Path.Combine(Directory, saveSlotID, SubFolders, FileName);
			string parentDirectory = Path.GetDirectoryName(fullPath);
			try
			{
				if (!System.IO.Directory.Exists(parentDirectory))
					System.IO.Directory.CreateDirectory(parentDirectory);

				// Serialize the object's data into a json formatted string.
				string serializedData = JsonConvert.SerializeObject(dataToSave, Formatting.Indented);

				// Encrypt the data (Optional).
				if (UseEncryption)
					serializedData = EncryptOrDecrypt(serializedData);

				// Write the serialized data to file.
				using (FileStream file = new FileStream(fullPath, FileMode.Create))
				{
					using StreamWriter writer = new StreamWriter(file);
					{
						writer.Write(serializedData);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error occured when trying to save data to file.\n" +
							   $"At full path: {fullPath}.\n" +
							   $"Reason: {ex.Message}.");
			}
		}

		/// <summary>
		/// Save the data of type TData using Newtonsoft's JSON.Net.
		/// </summary>
		/// <param name="dataToSave"> The data object to save. </param>
		public void SaveDataToFile(TData dataToSave)
		{
			string fullPath = Path.Combine(Directory, SubFolders, FileName);
			string parentDirectory = Path.GetDirectoryName(fullPath);

			try
			{
				if (!System.IO.Directory.Exists(parentDirectory))
					System.IO.Directory.CreateDirectory(parentDirectory);

				// Serialize data.
				string serializedData = JsonConvert.SerializeObject(dataToSave, Formatting.Indented);

				// Encrypt the data (Optional).
				if (UseEncryption)
					serializedData = EncryptOrDecrypt(serializedData);

				// Write the serialized data to file.
				using (FileStream file = new FileStream(fullPath, FileMode.Create))
				{
					using StreamWriter writer = new StreamWriter(file);
					{
						writer.Write(serializedData);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error occured when trying to save json to file.\n" +
							   $"At full path: {fullPath}.\n" +
							   $"Reason: {ex.Message}.");
			}
		}

		private string EncryptOrDecrypt(string data)
		{
			string modifiedData = "";
			int codeLength = ENCRYPTION_CODE.Length;

			for (int i = 0; i < data.Length; i++)
			{
				// This will cycle back and forth the encryption code.
				modifiedData += (char)(data[i] ^ ENCRYPTION_CODE[i % codeLength]);
			}

			return modifiedData;
		}
	}
}