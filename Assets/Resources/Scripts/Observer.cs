using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;

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
        [Tooltip("check if you are not going to play but watch"), SerializeField] private bool _isPlaying;
        //[SerializeField] private bool _isWriting;



        private static string path = Environment.CurrentDirectory + @"\Assets\Resources\Scripts\CheckersHistory.txt";
        private IDisposable _unsubscriber;

        public delegate void ObserverReadHandler(string playerColor, string componentName, bool isDestroyed, string whereToMove);
        public static event ObserverReadHandler OnObserverRead;

        private void OnEnable()
        {
            Player.OnObserverWrite += WriteInfo;
            if (!_isPlaying)
                File.WriteAllText(path, string.Empty);
            else
                PlayInfo();
        }

        private void OnDisable()
        {
            Player.OnObserverWrite -= WriteInfo;
        }

        public void OnCompleted()
        {
            print("The Observer has completed writing info.");
            this.Unsubscribe();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Player player)
        {
            throw new NotImplementedException();
        }

        public virtual void Subscribe(IObservable<Player> player)
        {
            if (player != null)
                _unsubscriber = player.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }

        private void Awake()
        {
            
        }

        static async Task WriteInfo(ColorType color, string chipInfo, ChipCondition chipcondition, string cellInfo)
        {
                using (StreamWriter stream = new StreamWriter(path, true))
                {
                    await Task.Run(() =>
                    {
                        stream.WriteLine("{0} chip {1} was {2} at {3}", color, chipInfo, chipcondition, cellInfo);
                      
                    });
                    //await stream.WriteAllLinesAsync(write);
                    //stream.Write("{0} chip was {1} at {2}", color, chipcondition, cellInfo);
                    //stream.WriteLine();
                    //stream.WriteLine("Started");
                }
        }

        private async void PlayInfo()
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    string[] words = line.Split(' ');
                    bool destroy = (words[4] == "Destroyed");
                    string whereToMove = words[4] == "Moved" ? words[6] : null;
                    /*
                    Debug.Log(words[0]);
                    Debug.Log(words[2]);
                    Debug.Log(destroy);
                    Debug.Log(whereToMove);*/

                    OnObserverRead?.Invoke(words[0], words[2], destroy, whereToMove);

                    
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
