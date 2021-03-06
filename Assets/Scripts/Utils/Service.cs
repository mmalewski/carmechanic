﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SQLite4Unity3d;
using SimpleJSON;

public class Service
{
	public static Dictionary<CarEngine.Part, PartStaticInfo> partsList = new Dictionary<CarEngine.Part, PartStaticInfo>();
	public static Dictionary<string, CarInfo> carList = new Dictionary<string, CarInfo>();
	public static SQLiteConnection _db;
	public static SQLiteConnection db {
		get { 
			if (_db == null)
			{
				string DatabaseName = "CarMechanic.db";
				string dbPath;

				#if UNITY_EDITOR
					dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
				#else
					// check if file exists in Application.persistentDataPath
					dbPath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

					if (!File.Exists(dbPath)){
						//string loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;
						string loadDb = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
					
						// then save to Application.persistentDataPath
						File.Copy(loadDb, dbPath);
					}
				#endif

				_db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

			}
			return _db;
		}
		set {}
	}

	public static void resetDB () {
		//@ToDo replace CarMechanic.db with CarMechanicOriginal.db and execute this method before git commiting
	}

	public static T getOne<T> (System.Linq.Expressions.Expression<System.Func<T, bool>> predExpr) where T:class,new()
	{
		try {
			return Service.db.Table<T>().Where(predExpr).FirstOrDefault();
		} catch (System.Exception) {
			return null;
		}
	}

	// Parses all jsons to arrays
	public static void init ()
	{
		int i = 0;
		JSONNode list = Utils.getJSON("Translation/parts");
		JSONNode item;

		while (i < list.Count)
		{
			item = list[i];
			
			partsList.Add((CarEngine.Part)item["id"].AsInt, new PartStaticInfo{
				name = item["name"].Value,
				description = item["description"].Value,
				price = item["price"].AsInt
			});
			i++;
		}
		/*
		i = 0;
		list = Utils.getJSON("vehicles");

		while (i < list.Count)
		{
			item = list[i];

			carList.Add(item["folder"].Value, new CarInfo{
				name = item["name"].Value,
				folder = item["folder"].Value,
			});
			i++;
		}*/
	}
}