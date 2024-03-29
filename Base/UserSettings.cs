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

using System.Collections.Generic;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

namespace BluRip
{
    public class LanguageInfo
    {
        public LanguageInfo() { }

        public LanguageInfo(string language, string translation, string languageShort)
        {
            this.language = language;
            this.translation = translation;
            this.languageShort = languageShort;
        }

        public LanguageInfo(LanguageInfo orig)
        {
            this.language = orig.language;
            this.translation = orig.translation;
            this.languageShort = orig.languageShort;
        }

        public string language = "";
        public string translation = "";
        public string languageShort = "";
    }

    public enum SizeType
    {
        Bitrate, // specify bitrate
        Size // specify target size
    }
        
    public class EncodingSettings
    {
        public EncodingSettings() { }

        public EncodingSettings(EncodingSettings orig)
        {
            this.desc = orig.desc;
            this.settings = orig.settings;
            this.settings2 = orig.settings2;
            this.pass2 = orig.pass2;
            this.sizeValue = orig.sizeValue;
            this.sizeType = orig.sizeType;

            this.crf = orig.crf;
            this.profile = orig.profile;
            this.preset = orig.preset;
            this.tune = orig.tune;
            this.level = orig.level;
            this.refvalue = orig.refvalue;
            this.bframes = orig.bframes;
            this.badapt = orig.badapt;
            this.aqmode = orig.aqmode;
            this.nofastpskip = orig.nofastpskip;
            this.fastdecode = orig.fastdecode;
            this.zerolatency = orig.zerolatency;
            this.slowfirstpass = orig.slowfirstpass;
        }

        public EncodingSettings(string desc, string parameter)
        {
            this.desc = desc;
            this.settings = parameter;
        }

        public EncodingSettings(string desc, string parameter, string parameter2, bool pass2)
        {
            this.desc = desc;
            this.settings = parameter;
            this.settings2 = parameter2;
            this.pass2 = pass2;
        }

        public EncodingSettings(string desc, string parameter, string parameter2, bool pass2, double sizeValue, SizeType sizeType)
        {
            this.desc = desc;
            this.settings = parameter;
            this.settings2 = parameter2;
            this.pass2 = pass2;
            this.sizeValue = sizeValue;
            this.sizeType = sizeType;
        }

        public EncodingSettings(string desc, double crf)
        {
            this.desc = desc;
            this.crf = crf;
        }

        public EncodingSettings(string desc, double crf, int tune, int aqmode)
        {
            this.desc = desc;
            this.crf = crf;
            this.tune = tune;
            this.aqmode = aqmode;
        }

        public EncodingSettings(string desc, double sizevalue, SizeType sizeType, int tune, int aqmode)
        {
            this.desc = desc;
            this.sizeValue = sizevalue;
            this.sizeType = sizeType;
            this.pass2 = true;            
            this.tune = tune;
            this.aqmode = aqmode;
        }

        public string desc = "";
        public string settings = "";
        public bool pass2 = false;
        public string settings2 = "";
        public double sizeValue = 0;
        public SizeType sizeType = SizeType.Bitrate;

        public double crf = 18.0;
        public int profile = 3;
        public int preset = 7;
        public int tune = 1;
        public int level = 12;
        public int refvalue = 4; // default maximum for 1080p
        public int bframes = 0;
        public int badapt = 0;
        public int aqmode = 0;
        public bool nofastpskip = true;
        public bool fastdecode = false;
        public bool zerolatency = false;
        public bool slowfirstpass = false;

        [XmlIgnore]
        public string GetParam
        {
            get
            {
                string tmp = "";
                if (profile > 0 && profile < GlobalVars.profile.Count) tmp += "--profile " + GlobalVars.profile[profile] + " ";
                if (preset > 0 && preset < GlobalVars.preset.Count) tmp += "--preset " + GlobalVars.preset[preset] + " ";
                List<string> tunePara = new List<string>();
                if (tune > 0 && tune < GlobalVars.tune.Count) tunePara.Add(GlobalVars.tune[tune]);
                if(fastdecode) tunePara.Add("fastdecode");
                if(zerolatency) tunePara.Add("zerolatency");
                if(tunePara.Count > 0)
                {
                    string tmp2 = "";
                    for(int i=0; i < tunePara.Count; i++)
                    {
                        tmp2 += tunePara[i];
                        if( i < tunePara.Count - 1) tmp2 += ",";
                    }
                    tmp += "--tune " + tmp2 + " ";
                }                
                if (level > 0 && level < GlobalVars.level.Count) tmp += "--level " + GlobalVars.level[level] + " ";
                if (aqmode > 0 && aqmode < GlobalVars.aqmode.Count) tmp += "--aq-mode " + GlobalVars.aqmode[aqmode] + " ";
                if (badapt > 0 && badapt < GlobalVars.badapt.Count) tmp += "--b-adapt " + GlobalVars.badapt[badapt] + " ";
                if (!pass2)
                {
                    tmp += "--crf " + crf.ToString("f2", CultureInfo.InvariantCulture.NumberFormat) + " ";
                }
                if (bframes > 0) tmp += "--bframes " + bframes.ToString() + " ";
                if (refvalue > 0) tmp += "--ref " + refvalue.ToString() + " ";
                if (tmp.Length > 0 && tmp[tmp.Length - 1] != ' ') tmp += " ";
                if (nofastpskip) tmp += "--no-fast-pskip ";
                if (pass2)
                {
                    if (slowfirstpass) tmp += "--slow-firstpass ";
                    tmp += "--pass 1 --bitrate {0} --stats \"{1}\" ";
                }

                tmp += settings;
                return tmp;
            }
        }

        [XmlIgnore]
        public string GetSecondParam
        {
            get
            {
                if (!pass2) return "";
                string tmp = "";
                if (profile > 0 && profile < GlobalVars.profile.Count) tmp += "--profile " + GlobalVars.profile[profile] + " ";
                if (preset > 0 && preset < GlobalVars.preset.Count) tmp += "--preset " + GlobalVars.preset[preset] + " ";
                List<string> tunePara = new List<string>();
                if (tune > 0 && tune < GlobalVars.tune.Count) tunePara.Add(GlobalVars.tune[tune]);
                if (fastdecode) tunePara.Add("fastdecode");
                if (zerolatency) tunePara.Add("zerolatency");
                if (tunePara.Count > 0)
                {
                    string tmp2 = "";
                    for (int i = 0; i < tunePara.Count; i++)
                    {
                        tmp2 += tunePara[i];
                        if (i < tunePara.Count - 1) tmp2 += ",";
                    }
                    tmp += "--tune " + tmp2 + " ";
                }
                if (level > 0 && level < GlobalVars.level.Count) tmp += "--level " + GlobalVars.level[level] + " ";
                if (aqmode > 0 && aqmode < GlobalVars.aqmode.Count) tmp += "--aq-mode " + GlobalVars.aqmode[aqmode] + " ";
                //if (badapt > 0 && badapt < GlobalVars.badapt.Count) tmp += "--b-adapt " + GlobalVars.badapt[badapt] + " ";
                if (!pass2)
                {
                    tmp += "--crf " + crf.ToString("f2") + " ";
                }
                if (bframes > 0) tmp += "--bframes " + bframes.ToString() + " ";
                if (refvalue > 0) tmp += "--ref " + refvalue.ToString() + " ";
                if (tmp.Length > 0 && tmp[tmp.Length - 1] != ' ') tmp += " ";
                if (nofastpskip) tmp += "--no-fast-pskip ";
                if (pass2)
                {
                    tmp += "--pass 2 --bitrate {0} --stats \"{1}\" ";
                }

                tmp += settings2;
                return tmp;
            }
        }
    }

    public class AvisynthSettings
    {
        public AvisynthSettings() { }

        public AvisynthSettings(string desc, string commands)
        {
            this.desc = desc;
            this.commands = commands;
        }

        public AvisynthSettings(AvisynthSettings orig)
        {
            this.desc = orig.desc;
            this.commands = orig.commands;
        }

        public string desc = "";

        public string commands = "";
    }

    public class UserSettings
    {
        public UserSettings() { }

        public UserSettings(UserSettings orig)
        {
            this.eac3toPath = orig.eac3toPath;
            this.lastBluRayPath = orig.lastBluRayPath;
            this.useAutoSelect = orig.useAutoSelect;
            this.includeChapter = orig.includeChapter;
            this.includeSubtitle = orig.includeSubtitle;
            this.preferDTS = orig.preferDTS;
            this.preferredAudioLanguages.Clear();
            this.workingDir = orig.workingDir;
            this.encodedMovieDir = orig.encodedMovieDir;
            this.ffmsindexPath = orig.ffmsindexPath;
            this.x264Path = orig.x264Path;
            this.sup2subPath = orig.sup2subPath;
            this.nrFrames = orig.nrFrames;
            this.blackValue = orig.blackValue;
            this.filePrefix = orig.filePrefix;
            this.javaPath = orig.javaPath;            
            this.cropMode = orig.cropMode;
            this.mkvmergePath = orig.mkvmergePath;
            this.x264Priority = orig.x264Priority;
            this.targetFolder = orig.targetFolder;
            this.targetFilename = orig.targetFilename;
            this.movieTitle = orig.movieTitle;
            this.defaultAudio = orig.defaultAudio;
            this.defaultSubtitle = orig.defaultSubtitle;
            this.defaultSubtitleForced = orig.defaultSubtitleForced;
            this.defaultForcedFlag = orig.defaultForcedFlag;
            this.lastProfile = orig.lastProfile;
            this.dtsHdCore = orig.dtsHdCore;
            this.untouchedVideo = orig.untouchedVideo;
            this.lastAvisynthProfile = orig.lastAvisynthProfile;
            this.resize720p = orig.resize720p;
            this.downmixAc3 = orig.downmixAc3;
            this.downmixAc3Index = orig.downmixAc3Index;
            this.downmixDTS = orig.downmixDTS;
            this.downmixDTSIndex = orig.downmixDTSIndex;
            this.minimizeAutocrop = orig.minimizeAutocrop;
            this.cropInput = orig.cropInput;
            this.encodeInput = orig.encodeInput;
            this.untouchedAudio = orig.untouchedAudio;
            this.muxSubs = orig.muxSubs;
            this.copySubs = orig.copySubs;
            this.dgindexnvPath = orig.dgindexnvPath;
            this.convertDtsToAc3 = orig.convertDtsToAc3;
            this.x264x64Path = orig.x264x64Path;
            this.avs2yuvPath = orig.avs2yuvPath;
            this.use64bit = orig.use64bit;
            this.muxLowResSubs = orig.muxLowResSubs;
            this.deleteIndex = orig.deleteIndex;
            this.muxUntouchedSubs = orig.muxUntouchedSubs;
            this.copyUntouchedSubs = orig.copyUntouchedSubs;
            this.deleteAfterEncode = orig.deleteAfterEncode;
            this.doDemux = orig.doDemux;
            this.doIndex = orig.doIndex;
            this.doSubtitle = orig.doSubtitle;
            this.doEncode = orig.doEncode;
            this.doMux = orig.doMux;
            this.suptitlePath = orig.suptitlePath;
            this.autoScroll = orig.autoScroll;

            this.disableAudioHeaderCompression = orig.disableAudioHeaderCompression;
            this.disableVideoHeaderCompression = orig.disableVideoHeaderCompression;
            this.disableSubtitleHeaderCompression = orig.disableSubtitleHeaderCompression;
            this.resizeMethod = orig.resizeMethod;
            this.manualCrop = orig.manualCrop;
            this.addAc3ToAllDts = orig.addAc3ToAllDts;

            this.snap = orig.snap;
            this.expertMode = orig.expertMode;
            this.showLog = orig.showLog;
            this.logX = orig.logX;
            this.logY = orig.logY;
            this.logHeight = orig.logHeight;
            this.logWidth = orig.logWidth;
            this.showDemuxedStream = orig.showDemuxedStream;
            this.demuxedStreamsX = orig.demuxedStreamsX;
            this.demuxedStreamsY = orig.demuxedStreamsY;
            this.demuxedStreamsHeight = orig.demuxedStreamsHeight;
            this.demuxedStreamsWidth = orig.demuxedStreamsWidth;
            this.showQueue = orig.showQueue;
            this.queueX = orig.queueX;
            this.queueY = orig.queueY;
            this.queueHeight = orig.queueHeight;
            this.queueWidth = orig.queueWidth;
            this.bluripX = orig.bluripX;
            this.bluripY = orig.bluripY;
            this.bluripHeight = orig.bluripHeight;
            this.bluripWidth = orig.bluripWidth;
            this.language = orig.language;
            this.skin = orig.skin;
            
            this.preferredAudioLanguages.Clear();
            this.preferredSubtitleLanguages.Clear();
            this.encodingSettings.Clear();
            this.avisynthSettings.Clear();

            foreach (LanguageInfo li in orig.preferredAudioLanguages)
            {
                this.preferredAudioLanguages.Add(new LanguageInfo(li));
            }

            foreach (LanguageInfo li in orig.preferredSubtitleLanguages)
            {
                this.preferredSubtitleLanguages.Add(new LanguageInfo(li));
            }

            foreach (EncodingSettings es in orig.encodingSettings)
            {
                this.encodingSettings.Add(new EncodingSettings(es));
            }

            foreach(AvisynthSettings avs in orig.avisynthSettings)
            {
                this.avisynthSettings.Add(new AvisynthSettings(avs));
            }
        }
             

        public static bool SaveSettingsFile(UserSettings settings, string filename)
        {
            MemoryStream ms = null;
            FileStream fs = null;
            XmlSerializer xs = null;
            
            try
            {
                ms = new MemoryStream();
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write);

                xs = new XmlSerializer(typeof(UserSettings));
                xs.Serialize(ms, settings);
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

        public static bool LoadSettingsFile(ref UserSettings settings, string filename)
        {
            MemoryStream ms = null;
            
            try
            {
                if (!File.Exists(filename)) return false;
                byte[] data = File.ReadAllBytes(filename);
                XmlSerializer xs = new XmlSerializer(typeof(UserSettings));
                ms = new MemoryStream(data);
                ms.Seek(0, SeekOrigin.Begin);
                
                settings = (UserSettings)xs.Deserialize(ms);
                ms.Close();

                if (settings.preferredAudioLanguages.Count == 0)
                {
                    //settings.preferredAudioLanguages.Add(new LanguageInfo("German", "Deutsch", "de"));
                    settings.preferredAudioLanguages.Add(new LanguageInfo("English","Englisch","en"));
                }

                if (settings.preferredSubtitleLanguages.Count == 0)
                {
                    //settings.preferredSubtitleLanguages.Add(new LanguageInfo("German", "Deutsch", "de"));
                    settings.preferredSubtitleLanguages.Add(new LanguageInfo("English", "Englisch", "en"));
                }

                if(settings.encodingSettings.Count == 0)
                {
                    settings.encodingSettings.Add(new EncodingSettings("Crf 18.0/tune film",18.0));
                    settings.encodingSettings.Add(new EncodingSettings("Crf 19.0/tune film", 19.0));
                    settings.encodingSettings.Add(new EncodingSettings("Crf 20.0/tune film", 20.0));

                    settings.encodingSettings.Add(new EncodingSettings("Crf 18.0/tune animation", 18.0, 2, 1));
                    settings.encodingSettings.Add(new EncodingSettings("Crf 19.0/tune animation", 19.0, 2, 1));
                    settings.encodingSettings.Add(new EncodingSettings("Crf 20.0/tune animation", 20.0, 2, 1));

                    settings.encodingSettings.Add(new EncodingSettings("2-pass 8000kbps/tune film", 8000, SizeType.Bitrate, 1, 0));
                    settings.encodingSettings.Add(new EncodingSettings("2-pass 8000kbps/tune animation", 8000, SizeType.Bitrate, 2, 1));

                    settings.encodingSettings.Add(new EncodingSettings("2-pass 10000Mb/tune film", 10000, SizeType.Size, 1, 0));
                    settings.encodingSettings.Add(new EncodingSettings("2-pass 10000Mb/tune animation", 10000, SizeType.Size, 2, 1));
                }

                if (settings.avisynthSettings.Count == 0)
                {
                    settings.avisynthSettings.Add(new AvisynthSettings("Empty", ""));
                    settings.avisynthSettings.Add(new AvisynthSettings("Undot", "# undot - remove minimal noise\r\nUndot()\r\n"));
                }

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

        public string eac3toPath = "";
        public string lastBluRayPath = "";
        public bool useAutoSelect = true;
        public bool includeChapter = true;
        public bool includeSubtitle = true;
        public bool preferDTS = true;        
        public List<LanguageInfo> preferredAudioLanguages = new List<LanguageInfo>();
        public List<LanguageInfo> preferredSubtitleLanguages = new List<LanguageInfo>();
        public string workingDir = "";
        public string encodedMovieDir = "";
        public string ffmsindexPath = "";
        public string x264Path = "";
        public string sup2subPath = "";
        public int nrFrames = 10;
        public int blackValue = 50000;
        public string filePrefix = "";
        public string javaPath = "";        
        public List<EncodingSettings> encodingSettings = new List<EncodingSettings>();        
        public int cropMode = 0;
        public string mkvmergePath = "";
        public System.Diagnostics.ProcessPriorityClass x264Priority = System.Diagnostics.ProcessPriorityClass.Normal;
        public string targetFolder = "";
        public string targetFilename = "";
        public string movieTitle = "";
        public bool defaultAudio = true;
        public bool defaultSubtitle = true;
        public bool defaultSubtitleForced = true;
        public bool defaultForcedFlag = true;
        public int lastProfile = 0;
        public bool deleteAfterEncode = false;
        public bool dtsHdCore = true;        
        public bool untouchedVideo = false;
        public List<AvisynthSettings> avisynthSettings = new List<AvisynthSettings>();
        public int lastAvisynthProfile = 0;
        public bool resize720p = false;
        public bool downmixDTS= false;
        public int downmixDTSIndex = 0;
        public bool downmixAc3 = false;
        public int downmixAc3Index = 0;
        public bool minimizeAutocrop = false;
        public int cropInput = 1;
        public int encodeInput = 1;
        public bool untouchedAudio = false;
        public int muxSubs = 1;
        public int copySubs = 1;
        public string dgindexnvPath = "";
        public bool convertDtsToAc3 = false;
        public string x264x64Path = "";
        public string avs2yuvPath = "";
        public bool use64bit = false;
        public bool muxLowResSubs = false;
        public bool deleteIndex = false;
        public bool muxUntouchedSubs = false;
        public bool copyUntouchedSubs = false;
        public string suptitlePath = "";

        public bool doDemux = true;
        public bool doIndex = true;
        public bool doSubtitle = true;
        public bool doEncode = true;
        public bool doMux = true;
        public bool autoScroll = true;

        public bool disableAudioHeaderCompression = false;
        public bool disableVideoHeaderCompression = false;
        public bool disableSubtitleHeaderCompression = false;
        public int resizeMethod = 4;
        public bool manualCrop = false;
        public bool addAc3ToAllDts = false;
        // window settings

        public bool snap = false;        
        public bool expertMode = false;

        public bool showLog = false;
        public double logX = 80;
        public double logY = 80;
        public double logHeight = 300;
        public double logWidth = 700;

        public bool showDemuxedStream = false;
        public double demuxedStreamsX = 100;
        public double demuxedStreamsY = 100;
        public double demuxedStreamsHeight = 300;
        public double demuxedStreamsWidth = 550;

        public bool showQueue = false;
        public double queueX = 120;
        public double queueY = 120;
        public double queueHeight = 400;
        public double queueWidth = 400;

        public double bluripX = 140;
        public double bluripY = 140;
        public double bluripHeight = 800;
        public double bluripWidth = 600;

        public string language = "en";
        public string skin = "blu";
    }
}