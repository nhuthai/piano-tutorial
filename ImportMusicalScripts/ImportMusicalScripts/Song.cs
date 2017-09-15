using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportMusicalScripts
{
    class Song
    {
        public string name { get; set; }
        public List<List<int>> duration { get; set; }

        public Song()
        {
            this.name = "";
            this.duration = new List<List<int>>();
        }

        public Song(string name)
        {
            this.name = name;
            this.duration = new List<List<int>>();
        }

        public Song(List<List<int>> the_duration)
        {
            this.name = "";
            this.duration = the_duration;
        }

        public Song(string name, List<List<int>> the_duration)
        {
            this.name = name;
            this.duration = the_duration;
        }

        public void addDuration(List<int> the_dura)
        {
            duration.Add(the_dura);
        }

        public List<int> searchDuration(int time)
        {
            List<int> tmp = null;
            foreach (List<int> aDur in duration)
            {
                if (aDur[0] == time)
                    tmp = aDur;
            }
            return tmp;
        }

        public void modifyKey(int time, int new_key)
        {
            List<int> tmp = searchDuration(time);
            tmp[1] = new_key;
        }

        public void modifyColor(int time, int new_color)
        {
            List<int> tmp = searchDuration(time);
            tmp[2] = new_color;
        }

        public void removeDuration(List<int> the_dur)
        {
            if (the_dur != null)
                duration.Remove(the_dur);
        }

        public void removeDuration(int time)
        {
            List<int> removed_dur = searchDuration(time);
            removeDuration(removed_dur);
        }
    }
}
