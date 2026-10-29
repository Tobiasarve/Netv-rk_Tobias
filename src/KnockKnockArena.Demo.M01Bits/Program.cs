using System;
using KnockKnockArena.Shared.Protocol.Udp;


Console.WriteLine("Same eightbits, two types");

byte allOnes = 0b1111_1111;
sbyte asSigned = unchecked((sbyte)allOnes);

Console.WriteLine($"{WeaponBits.ToBinaryString(allOnes)} -> byte {allOnes}, sbyte{asSigned}");



//MovementBits keys = MovementBits.W | MovementBits.D | MovementBits.Sprint;

//Console.WriteLine($"Keys held: W + D + Shift -> movement byte {WeaponBits.ToBinaryString((byte)keys)}= 0X{(byte)keys:X2} ={(byte)keys}");


//Console.WriteLine(
//    $"[Flags] prints it as \"{keys}\"; " +
//    $"Is Sprint set? {(keys & MovementBits.Sprint) != 0}; " +
//    $"Is S set? {(keys & MovementBits.S) != 0}"
//);
//MovementBits released = keys & ~MovementBits.Sprint;

//Console.WriteLine($" release Shift (AND NOT Sprint); {MovementBits.ToBinaryString((byte)released)} =\"{released}\"");

//ButtonBits buttons = (ButtonBits)0b0000_0011;

//Console.WriteLine($" a buttons of 00000011 means \"{buttons}\" - both travel in ThreadExceptionEventArgs 22 byte Input packet (M06)");






//byte owned = WeaponBits.StartingWeapons;

//Show("Spawning (Punch + Glock)", owned);

//owned = WeaponBits.AddWeapon(owned, WeaponId.Ak47);

//Show("Picked up AK47", owned);

//owned = WeaponBits.AddWeapon(owned, WeaponId.RocketLauncher);

//Show("Picked up RocketLauncher", owned);

//owned = WeaponBits.AddWeapon(owned, WeaponId.Ak47);

//Show("Picked up another AK47", owned);

//owned = WeaponBits.RemoveWeapon(owned, WeaponId.RocketLauncher);

//Show("Removed rocketlauncher", owned);

//owned = WeaponBits.StartingWeapons;

//Show("Respawned with starting weapons", owned);


////Console.WriteLine(WeaponBits.HasWeapon(WeaponBits.StartingWeapons, WeaponId.Glock));
////Console.WriteLine(WeaponBits.HasWeapon(WeaponBits.StartingWeapons, WeaponId.AK47));
////Console.WriteLine(WeaponBits.ToBinaryString(WeaponBits.AddWeapon(WeaponBits.StartingWeapons,WeaponId.AK47)));

//void Show (string label, byte value)
//{
//    List<string> names = new List<string>();

//    foreach (WeaponId w in Enum.GetValues(typeof(WeaponId)))
//    {
//        if (WeaponBits.HasWeapon(value, w)) names.Add(w.ToString());        
//    }

//    Console.WriteLine($"{label,-30} {WeaponBits.ToBinaryString(value)} "+ $"0x{value:X2} {value,3} [{string.Join(",", names)}]");
//}


