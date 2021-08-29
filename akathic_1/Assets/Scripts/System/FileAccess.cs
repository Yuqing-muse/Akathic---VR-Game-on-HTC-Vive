using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
public interface DataElememt<T> where T :new()
{
	string DataToFile(T _t);
	T FileToData(string _s);
	T Instance ();
};
public class MyData
{
	public Vector3 pos;
	public int gameTime;
	public int hp;
	public int weaponIndex;
	public int[] items;
	public bool[] bossDied;
	public int itemAll;
	public int bossAll;
	public MyData()
	{
		pos = Vector3.zero;
		gameTime = 0;
		hp = 200;
		weaponIndex = 0;
		items = new int[10];
		bossDied = new bool[10];
		itemAll = 0;
		bossAll = 3;
	}
	public  static MyData StringtoData(string _s)
	{
		MyData _md=new MyData();
		string posX = _s.Substring (0,6);
		string posY = _s.Substring (6,6);
		string posZ = _s.Substring (12,6);
		Vector3 pos = new Vector3 (String2float(posX),String2float(posY),String2float(posZ));
		_md.pos = pos;
		string gameTime_ = _s.Substring (18,7);
		_md.gameTime = String2Int (gameTime_);
		string hp_ = _s.Substring (25,7);
		_md.hp = String2Int (hp_);
		string weaponIndex_ = _s.Substring (32,7);
		_md.weaponIndex = String2Int (weaponIndex_);
		_md.itemAll = String2Int (_s.Substring(39,7));
		for(int i=0;i<_md.itemAll;i++)
		{
			_md.items[i] = String2Int (_s.Substring(46+i*7,7));
		}
		_md.bossAll = String2Int (_s.Substring (46 + _md.itemAll * 7, 7));
		for (int i = 0; i < _md.bossAll; i++)
		{
			string tempBoss = _s.Substring (53+_md.itemAll*7+i,1);
			if (tempBoss == "1")
				_md.bossDied [i] = true;
			else
				_md.bossDied [i] = false;
		}
		return _md;
	}
	public static string dataToString(MyData _md)
	{
		string _result="";
		string posX = float2String(_md.pos.x);
		string posY = float2String (_md.pos.y);
		string posZ = float2String (_md.pos.z);
		string gameTime_ = Int2String (_md.gameTime);
		string hp_ = Int2String (_md.hp);
		string weaponIndex_ = Int2String (_md.weaponIndex);
		_result += posX;
		_result += posY;
		_result += posZ;
		_result += gameTime_;
		_result += hp_;
		_result += weaponIndex_;
		string itemAll_ = Int2String (_md.itemAll);
		string bossAll_ = Int2String (_md.bossAll);
		_result += itemAll_;
		for(int i=0;i<_md.itemAll;i++)
		{
			string tempitem = Int2String (_md.items[i]);
			_result += tempitem;
		}
		_result += bossAll_;
		for(int i=0;i<_md.bossAll;i++)
		{
			string tempBossDied = "";
			if(_md.bossDied[i])
				tempBossDied+="1";
			else
				tempBossDied+="0";
			_result += tempBossDied;
		}
		Debug.Log (_result);
		return _result;
	}
	public static string float2String(float f)
	{
		string temp = "";
		int I = (int)(f * 100);
		for(int i=0;i<6;i++)
		{
			temp += (I % 10).ToString ();
			I /= 10;
		}
		return reverse(temp);
	}
	public static float String2float(string str)
	{
		int result = 0;
		for(int i=0;i<6;i++)
		{
			result += int.Parse(str.Substring(i,1)) * (int)Mathf.Pow (10, 5 - i);
		}
		float result_ = (float)result / 100.0f;
		return result_;
	}


	public static string Int2String(int x)
	{
		string _result="";
		for(int i=0;i<7;i++)
		{
			_result += (x % 10).ToString();
			x /= 10;
		}
		return reverse(_result);
	}
	public static string reverse(string s)
	{
		string _result="";
		for(int i=0;i<s.Length;i++)
		{
			_result += s [s.Length - 1 - i];
		}
		return _result;
	}
	public static int String2Int(string x)
	{
		int _result=0;
		for(int i=0;i<7;i++)
		{
			_result += int.Parse(x.Substring(i,1)) * (int)Mathf.Pow (10, 6 - i);
		}
		return _result;
	}
}
public class XMLDataTranslate<T>:DataElememt<T>where T :new()
{
	public delegate T del1(string _s);
	public delegate string del2(T _t);
	del1 _d1;
	del2 _d2;
	public T Instance()
	{
		return new T();
	}
	public string DataToFile(T _t)
	{
		return _d2 (_t);
	}
	public T FileToData(string _s)
	{
		return _d1 (_s);
	}
	public XMLDataTranslate(del1 d1,del2 d2)
	{
		_d1 = d1;
		_d2 = d2;
	}
}
public interface AccessStrategy<T>where T :new()
{
	string Access(T _t,DataElememt<T> dataElements);
	T Load(string s,DataElememt<T> dataElements);
};
public class DirectStrategy<T>:AccessStrategy<T>where T :new()
{
	public DirectStrategy()
	{
		
	}
	public string Access(T _data,DataElememt<T> _t)
	{
		return _t.DataToFile (_data);
	}
	public T Load(string s,DataElememt<T> _t)
	{
		return _t.FileToData (s);
	}
}
public class subaddStrategy<T>:AccessStrategy<T>where T:new()
{
	public subaddStrategy()
	{
		
	}
	public string Access(T _data,DataElememt<T> _t)
	{
		string _temp = _t.DataToFile (_data);
		string _result="";
		for(int i=0;i<_temp.Length;i++)
		{
			int index = _temp.Length-1-i;
			if (index % 2 == 0) 
			{
				char _tempchar = (char)((int)_temp [i] - 13);
				_result += _tempchar.ToString ();
			}
			else
			{
				char _tempchar = (char)((int)_temp [i] + 13);
				_result += _tempchar.ToString ();
			}
		}
		return _result;
	}
	public T Load(string s,DataElememt<T> _t)
	{
		string _real="";
		for(int i=0;i<s.Length;i++)
		{
			int index = s.Length - 1 - i;
			if(index%2==0)
			{
				char _tempchar = (char)((int)s [i] + 13);
				_real += _tempchar.ToString ();
			}
			else
			{
				char _tempchar = (char)((int)s [i] - 13);
				_real += _tempchar.ToString ();
			}
		}
		return _t.FileToData (_real);
	}
}	
public class DataManager<T>where T :new()
{
	AccessStrategy<T> _Strategy;
	string _file;
	DataElememt<T> _dataElement;
	public DataManager(AccessStrategy<T> strategy,DataElememt<T> dataElement,string file)
	{
		_Strategy = strategy;
		_file = file;
		_dataElement = dataElement;
	}
	public void Write(int _index,T _t)
	{
		XmlDocument xml_doc=new XmlDocument();
		string _tempMessage = _Strategy.Access (_t,_dataElement);		
		if (File.Exists (_file))
		{
			xml_doc.Load (_file);
			XmlNodeList	list_node=xml_doc.SelectSingleNode ("GAMEFILE").ChildNodes;
			bool isHaveIndex=false;
			string[] all_data=new string[list_node.Count];
			string[] all_index = new string[list_node.Count];
			int cp = 0;
			int cl = 0;
			foreach(XmlElement xn in list_node)
			{
				if (xn.InnerText == _index.ToString ()) 
				{
					isHaveIndex = true;
					cl = cp;
				}
				all_data [cp] = xn.GetAttribute ("data");
				all_index[cp] = xn.Name;
				cp++;
			}
			if (isHaveIndex) 
			{
				xml_doc = new XmlDocument ();
				XmlElement Top_ELEMENT = xml_doc.CreateElement ("GAMEFILE");
				xml_doc.AppendChild (Top_ELEMENT);
				for(int i=0;i<cp;i++)
				{
					if (i != cl)
					{
						XmlElement xe = xml_doc.CreateElement ("FILEDATA");
						xe.InnerText = all_index [i].ToString ();
						xe.SetAttribute ("data",all_data[i]);
						xml_doc.FirstChild.AppendChild (xe);
					}
				}
			}
		}
		else
		{
			XmlElement Top_ELEMENT = xml_doc.CreateElement ("GAMEFILE");
			xml_doc.AppendChild (Top_ELEMENT);
		}
		XmlElement xe1 = xml_doc.CreateElement ("FILEDATA");
		xe1.InnerText = _index.ToString ();
		xe1.SetAttribute ("data", _tempMessage);
		xml_doc.FirstChild.AppendChild (xe1);
		xml_doc.Save (_file);
	}
	public void Clear()
	{
		XmlDocument xml_doc=new XmlDocument();
		xml_doc.Save (_file);
	}
	public T Read(int _index)
	{
		XmlDocument xml_doc=new XmlDocument();
		xml_doc.Load (_file);
		T _result = _dataElement.Instance ();
		XmlNodeList	list_node=xml_doc.SelectSingleNode ("GAMEFILE").ChildNodes;
		foreach(XmlElement xn in list_node)
		{
			if (int.Parse (xn.InnerText) == _index)
			{
				string _temp;
				_temp = xn.GetAttribute ("data");
				_result = _Strategy.Load (_temp, _dataElement);
				break;
			}
		}
		return _result;
	}
};



