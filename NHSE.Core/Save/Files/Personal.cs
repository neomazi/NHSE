﻿using System;
using System.Collections.Generic;

namespace NHSE.Core
{
    /// <summary>
    /// personal.dat
    /// </summary>
    public sealed class Personal : EncryptedFilePair
    {
        public readonly PersonalOffsets Offsets;
        public Personal(string folder) : base(folder, "personal") => Offsets = PersonalOffsets.GetOffsets(Info);
        public override string ToString() => Name;

        public uint TownID
        {
            get => BitConverter.ToUInt32(Data, Offsets.PersonalId + 0x00);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offsets.PersonalId + 0x00);
        }

        public string TownName
        {
            get => GetString(Offsets.PersonalId + 0x04, 10);
            set => GetBytes(value, 10).CopyTo(Data, Offsets.PersonalId + 0x04);
        }

        public uint PlayerID
        {
            get => BitConverter.ToUInt32(Data, Offsets.PersonalId + 0x1C);
            set => BitConverter.GetBytes(value).CopyTo(Data, Offsets.PersonalId + 0x1C);
        }

        public string Name
        {
            get => GetString(Offsets.PersonalId + 0x20, 10);
            set => GetBytes(value, 10).CopyTo(Data, Offsets.PersonalId + 0x20);
        }

        public EncryptedInt32 Wallet
        {
            get => EncryptedInt32.Read(Data, Offsets.Wallet);
            set => value.Write(Data, Offsets.Wallet);
        }

        public EncryptedInt32 Bank
        {
            get => EncryptedInt32.Read(Data, Offsets.Bank);
            set => value.Write(Data, Offsets.Bank);
        }

        public EncryptedInt32 NookMiles
        {
            get => EncryptedInt32.Read(Data, Offsets.NookMiles);
            set => value.Write(Data, Offsets.NookMiles);
        }

        public IReadOnlyList<Item> Pocket1
        {
            get => Item.GetArray(Data.Slice(Offsets.Pockets1, Offsets.Pockets1Count * Item.SIZE));
            set => Item.SetArray(value).CopyTo(Data, Offsets.Pockets1);
        }

        public IReadOnlyList<Item> Pocket2
        {
            get => Item.GetArray(Data.Slice(Offsets.Pockets2, Offsets.Pockets2Count * Item.SIZE));
            set => Item.SetArray(value).CopyTo(Data, Offsets.Pockets2);
        }

        public IReadOnlyList<Item> Storage
        {
            get => Item.GetArray(Data.Slice(Offsets.Storage, Offsets.StorageCount * Item.SIZE));
            set => Item.SetArray(value).CopyTo(Data, Offsets.Storage);
        }

        public bool[] GetRecipeList() => ArrayUtil.GitBitFlagArray(Data, Offsets.Recipes, Offsets.MaxRecipeID + 1);
        public void SetRecipeList(bool[] value) => ArrayUtil.SetBitFlagArray(Data, Offsets.Recipes, value);

        public byte[] GetPhotoData()
        {
            var offset = Offsets.Photo;

            // Expect jpeg marker
            if (BitConverter.ToUInt16(Data, offset) != 0xD8FF)
                return Array.Empty<byte>();
            var len = BitConverter.ToInt32(Data, offset - 4);
            return Data.Slice(offset, len);
        }
    }
}