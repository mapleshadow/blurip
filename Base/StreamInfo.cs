﻿//BluRip - one click BluRay/m2ts to mkv converter
//Copyright (C) 2009-2010 _hawk_

//This program is free software; you can redistribute it and/or
//modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation; either version 2
//of the License, or (at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

//Contact: hawk.ac@gmx.net

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace BluRip
{
    [XmlInclude(typeof(AdvancedOptions))]
    [XmlInclude(typeof(AdvancedVideoOptions))]
    [XmlInclude(typeof(AdvancedAudioOptions))]
    [XmlInclude(typeof(AdvancedSubtitleOptions))]
    [XmlInclude(typeof(ExtraFileInfo))]
    [XmlInclude(typeof(VideoFileInfo))]
    [XmlInclude(typeof(SubtitleFileInfo))]
    [XmlInclude(typeof(CropInfo))]
    public class TitleInfo
    {
        public TitleInfo() { }

        public TitleInfo(TitleInfo orig)
        {
            this.desc = orig.desc;
            this.streamNumber = orig.streamNumber;
            this.streams.Clear();
            foreach (StreamInfo si in orig.streams)
            {
                this.streams.Add(new StreamInfo(si));
            }
            files.Clear();
            foreach (string s in orig.files)
            {
                this.files.Add(s);
            }
        }

        public string desc = "";
        public string streamNumber = "";
        public List<string> files = new List<string>();

        public List<StreamInfo> streams = new List<StreamInfo>();

        public static bool SaveStreamInfoFile(TitleInfo ti, string filename)
        {
            MemoryStream ms = null;
            FileStream fs = null;
            XmlSerializer xs = null;

            try
            {
                ms = new MemoryStream();
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write);

                xs = new XmlSerializer(typeof(TitleInfo));
                xs.Serialize(ms, ti);
                ms.Seek(0, SeekOrigin.Begin);

                fs.Write(ms.ToArray(), 0, (int)ms.Length);
                ms.Close();
                fs.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (ms != null) ms.Close();
                if (fs != null) fs.Close();
            }
        }

        public static bool LoadSettingsFile(ref TitleInfo ti, string filename)
        {
            MemoryStream ms = null;

            try
            {
                if (!File.Exists(filename)) return false;
                byte[] data = File.ReadAllBytes(filename);
                XmlSerializer xs = new XmlSerializer(typeof(TitleInfo));
                ms = new MemoryStream(data);
                ms.Seek(0, SeekOrigin.Begin);

                ti = (TitleInfo)xs.Deserialize(ms);
                ms.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (ms != null) ms.Close();
            }
        }
    }

    public enum StreamType
    {
        Unknown,
        Chapter,
        Video,
        Audio,
        Subtitle
    }

    public class ExtraFileInfo
    {
        public ExtraFileInfo() { }

        public ExtraFileInfo(ExtraFileInfo orig)
        {
        }
    }

    public class VideoFileInfo : ExtraFileInfo
    {
        public VideoFileInfo() { }

        public VideoFileInfo(ExtraFileInfo orig)
        {
            try
            {
                this.encodeAvs = ((VideoFileInfo)orig).encodeAvs;
                this.encodedFile = ((VideoFileInfo)orig).encodedFile;
                this.fps = ((VideoFileInfo)orig).fps;
                //this.resX = ((VideoFileInfo)orig).resX;
                //this.resY = ((VideoFileInfo)orig).resY;
                this.length = ((VideoFileInfo)orig).length;
                this.frames = ((VideoFileInfo)orig).frames;

                if (((VideoFileInfo)orig).cropInfo == null)
                {
                    this.cropInfo = null;
                }
                else
                {
                    this.cropInfo = new CropInfo(((VideoFileInfo)orig).cropInfo);
                }
            }
            catch (Exception)
            {
            }
        }

        public string encodeAvs = "";
        public string fps = "";
        public string encodedFile = "";
        //public string resX = "";
        //public string resY = "";
        public string length = "";
        public string frames = "";

        public CropInfo cropInfo = null;
    }

    public class SubtitleFileInfo : ExtraFileInfo
    {
        public SubtitleFileInfo() { }

        public SubtitleFileInfo(ExtraFileInfo orig)
        {
            try
            {
                this.forcedIdx = ((SubtitleFileInfo)orig).forcedIdx;
                this.forcedSub = ((SubtitleFileInfo)orig).forcedSub;
                this.forcedSup = ((SubtitleFileInfo)orig).forcedSup;

                this.normalIdx = ((SubtitleFileInfo)orig).normalIdx;
                this.normalSub = ((SubtitleFileInfo)orig).normalSub;
                this.normalSup = ((SubtitleFileInfo)orig).normalSup;

                this.forcedIdxLowRes = ((SubtitleFileInfo)orig).forcedIdxLowRes;
                this.forcedSubLowRes = ((SubtitleFileInfo)orig).forcedSubLowRes;

                this.normalIdxLowRes = ((SubtitleFileInfo)orig).normalIdxLowRes;
                this.normalSubLowRes = ((SubtitleFileInfo)orig).normalSubLowRes;

                this.isSecond = ((SubtitleFileInfo)orig).isSecond;
            }
            catch (Exception)
            {
            }
        }

        public string normalSub = "";
        public string normalIdx = "";
        public string normalSup = "";

        public string forcedSub = "";
        public string forcedIdx = "";
        public string forcedSup = "";

        public string normalSubLowRes = "";
        public string normalIdxLowRes = "";

        public string forcedSubLowRes = "";
        public string forcedIdxLowRes = "";

        public bool isSecond = false;
    }

    public class StreamInfo
    {
        public StreamInfo() { }

        public StreamInfo(StreamInfo orig)
        {
            try
            {
                this.addInfo = orig.addInfo;
                this.desc = orig.desc;
                this.filename = orig.filename;
                this.language = orig.language;
                this.number = orig.number;
                this.selected = orig.selected;
                this.streamType = orig.streamType;
                this.typeDesc = orig.typeDesc;
                Type extraFileInfoType = orig.extraFileInfo.GetType();
                Type advancedOptionsType = orig.advancedOptions.GetType();
                this.extraFileInfo = (ExtraFileInfo)Activator.CreateInstance(extraFileInfoType, orig.extraFileInfo);
                this.advancedOptions = (AdvancedOptions)Activator.CreateInstance(advancedOptionsType, orig.advancedOptions);
            }
            catch (Exception)
            {
            }
        }

        public int number = 0;
        public string typeDesc = "";
        public string desc = "";
        public string addInfo = "";
        public string language = "";
        public StreamType streamType = StreamType.Unknown;
        public bool selected = false;
        public string filename = "";        
        public ExtraFileInfo extraFileInfo = new ExtraFileInfo();
        public AdvancedOptions advancedOptions = new AdvancedOptions();
        
        [XmlIgnore]
        public int maxLength = 0;

        public string Desc
        {
            get 
            {
                string tmp = "[ " + number.ToString("d3") + " " + StreamTypeToString(streamType);
                for (int i = 0; i < maxLength - StreamTypeToString(streamType).Length; i++) tmp += " ";
                tmp += " ] - ";
                if (advancedOptions != null && advancedOptions.GetType() != typeof(AdvancedOptions)) tmp += "AO* ";
                tmp += "(" + desc + ")";
                if (addInfo != "")
                {
                    tmp += " - (" + addInfo + ")";
                }
                return tmp; 
            }
        }

        public bool Selected
        {
            get { return selected; }
        }

        private string StreamTypeToString(StreamType st)
        {
            if (st == StreamType.Audio)
            {
                return "AUDIO";
            }
            else if (st == StreamType.Chapter)
            {
                return "CHAPTER";
            }
            else if (st == StreamType.Subtitle)
            {
                return "SUBTITLE";
            }
            else if (st == StreamType.Unknown)
            {
                return "UNKNOWN";
            }
            else if (st == StreamType.Video)
            {
                return "VIDEO";
            }
            else
            {
                return "UNKNOWN";
            }
        }
    }

    public class AdvancedOptions
    {
        public AdvancedOptions() { }

        public AdvancedOptions(AdvancedOptions orig) { }
    }

    public class AdvancedAudioOptions : AdvancedOptions
    {
        public AdvancedAudioOptions() { }

        public AdvancedAudioOptions(AdvancedOptions orig)
        {
            this.bitrate = ((AdvancedAudioOptions)orig).bitrate;
            this.extension = ((AdvancedAudioOptions)orig).extension;
            this.parameter = ((AdvancedAudioOptions)orig).parameter;
            this.additionalAc3Track = ((AdvancedAudioOptions)orig).additionalAc3Track;
            this.additionalFilename = ((AdvancedAudioOptions)orig).additionalFilename;
        }

        public string extension = "";
        public string bitrate = "";
        public string parameter = "";
        public bool additionalAc3Track = false;
        public string additionalFilename = "";
    }

    public class AdvancedSubtitleOptions : AdvancedOptions
    {
        public AdvancedSubtitleOptions() { }

        public AdvancedSubtitleOptions(AdvancedOptions orig)
        {
            this.isForced = ((AdvancedSubtitleOptions)orig).isForced;
            this.supTitle = ((AdvancedSubtitleOptions)orig).supTitle;
            this.supTitleOnlyForced = ((AdvancedSubtitleOptions)orig).supTitleOnlyForced;
        }

        public bool isForced = false;
        public bool supTitle = false;
        public bool supTitleOnlyForced = false;
    }

    public class AdvancedVideoOptions : AdvancedOptions
    {
        public AdvancedVideoOptions() { }

        public AdvancedVideoOptions(AdvancedOptions orig)
        {
            this.disableFps = ((AdvancedVideoOptions)orig).disableFps;
            this.fps = ((AdvancedVideoOptions)orig).fps;
            this.length = ((AdvancedVideoOptions)orig).length;
            this.frames = ((AdvancedVideoOptions)orig).frames;

            this.disableAutocrop = ((AdvancedVideoOptions)orig).disableAutocrop;
            this.manualResize = ((AdvancedVideoOptions)orig).manualResize;
            this.resizeX = ((AdvancedVideoOptions)orig).resizeX;
            this.resizeY = ((AdvancedVideoOptions)orig).resizeY;
            this.resizeMethod = ((AdvancedVideoOptions)orig).resizeMethod;
            this.manualBorders = ((AdvancedVideoOptions)orig).manualBorders;
            this.borderBottom = ((AdvancedVideoOptions)orig).borderBottom;
            this.borderLeft = ((AdvancedVideoOptions)orig).borderLeft;
            this.borderRight = ((AdvancedVideoOptions)orig).borderRight;
            this.borderTop = ((AdvancedVideoOptions)orig).borderTop;
            this.manualCrop = ((AdvancedVideoOptions)orig).manualCrop;
            this.cropBottom = ((AdvancedVideoOptions)orig).cropBottom;
            this.cropLeft = ((AdvancedVideoOptions)orig).cropLeft;
            this.cropRight = ((AdvancedVideoOptions)orig).cropRight;
            this.cropTop = ((AdvancedVideoOptions)orig).cropTop;
            this.manualInputRes = ((AdvancedVideoOptions)orig).manualInputRes;
            this.inputResX = ((AdvancedVideoOptions)orig).inputResX;
            this.inputResY = ((AdvancedVideoOptions)orig).inputResY;
            this.noMkvDemux = ((AdvancedVideoOptions)orig).noMkvDemux;
            this.manualAspectRatio = ((AdvancedVideoOptions)orig).manualAspectRatio;
            this.aspectRatio = ((AdvancedVideoOptions)orig).aspectRatio;
        }

        public bool disableFps = false;
        public string fps = "";
        public string length = "";
        public string frames = "";

        public bool disableAutocrop = false;
        public bool manualResize = false;
        public int resizeX = 0;
        public int resizeY = 0;
        public int resizeMethod = 4;
        public bool manualBorders = false;
        public int borderLeft = 0;
        public int borderRight = 0;
        public int borderTop = 0;
        public int borderBottom = 0;
        public bool manualCrop = false;
        public int cropLeft = 0;
        public int cropRight = 0;
        public int cropTop = 0;
        public int cropBottom = 0;
        public bool manualInputRes = false;
        public int inputResX = 1920;
        public int inputResY = 1080;
        public bool noMkvDemux = false;
        public bool manualAspectRatio = false;
        public string aspectRatio = "";
    }
}