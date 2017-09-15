using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportMusicalScripts
{
    class SongList
    {
        public int n_songs { get; set; }
        public List<Song> songs { get; set; }

        public SongList() {
            n_songs = 0;
            songs = new List<Song>();
        }

        public SongList(List<Song> the_list)
        {
            songs = the_list;
            n_songs = the_list.Count;
        }

        public void addSong(Song the_song)
        {
            songs.Add(the_song);
            n_songs++;
        }

        public Song searchSong(string name_song)
        {
            Song tmp = null;
            foreach (Song aSong in songs)
            {
                if (aSong.name.Equals(name_song))
                    tmp = aSong;
            }
            return tmp;
        }

        public void removeSong(Song the_song)
        {
            if(the_song != null)
                songs.Remove(the_song);
            n_songs--;
        }

        public void removeSong(string name_song)
        {
            Song removed_song = searchSong(name_song);
            removeSong(removed_song);
        }

        /*public override string ToString()
        {
            string json_string = "{\"songs\":[";
            for(int i = 0; i < n_songs; i++)
            {
                json_string += mylist[i].ToString();
                if (i < n_songs - 1)
                    json_string += ",";
            }
            json_string += "]}";

            return json_string;
        }*/
    }
}
