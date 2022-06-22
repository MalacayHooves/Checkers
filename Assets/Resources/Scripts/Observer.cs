using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace Checkers
{
    public interface IWritingInfo
    {
        ChipCondition Condition { get; set; }
        string ChipNameInfo { get;}
        string CellNameInfo { get;}
    }
    public class Observer : MonoBehaviour, IObserver<Player>
    {
        [SerializeField] private bool _isReading;
        [SerializeField] private bool _isWriting;

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Player player)
        {
            throw new NotImplementedException();
        }

        private void Awake()
        {
            if (_isReading & _isWriting) Debug.LogError("Please, choose reading or writing only!");
        }

        private void WriteInfo()
        {
            using (var file = File.OpenWrite(Environment.CurrentDirectory + @"\Assets\Resources\Scripts\CheckersHistory.txt"))
            {
                using (var stream = new StreamWriter(file))
                {
                    stream.WriteLine("Started");
                }
            }

        }


    }

    public enum ChipCondition
    {
        Selected,
        Moved,
        Destroyed
    }
}
