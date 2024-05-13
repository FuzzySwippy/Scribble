using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Scribble.ScribbleLib;

public abstract class UniqueObject<T>
{
	#region Static
	static readonly HashSet<ulong> Objects = new();
	static readonly Random Randomizer = new();


	public static bool Exists(ulong id) => Objects.Contains(id);
	#endregion


	/// <summary>
	/// Unique identifier of the object
	/// </summary>
	[JsonIgnore]
	public ulong Id { get; }


	public UniqueObject()
	{
		Id = GetUniqueId();
		Objects.Add(Id);
	}


	static ulong GetUniqueId()
	{
		ulong id;

		do
			id = GenerateNewId();
		while (Objects.Contains(id));

		return id;
	}

	static ulong GenerateNewId()
	{
		ulong id = (ulong)DateTimeOffset.UtcNow.Ticks;
		ulong randomizer = 0;

		//Build randomizer value from two ints
		for (int i = 0; i < 2; i++)
			randomizer |= (ulong)Randomizer.Next(int.MinValue, int.MaxValue) << (i * 32);

		return id + randomizer;
	}


	public static bool operator ==(UniqueObject<T> a, UniqueObject<T> b) => a?.Id == b?.Id;

	public static bool operator !=(UniqueObject<T> a, UniqueObject<T> b) => a?.Id != b?.Id;


	public override bool Equals(object obj) => obj is UniqueObject<T> other && other.Id == Id;

	public override int GetHashCode() => Id.GetHashCode();

	~UniqueObject() => Objects.Remove(Id);
}
