﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipManager : MonoBehaviour {
  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "TooltipManager";
  private static TooltipManager _instance;
  public static TooltipManager get() {
    if(_instance == null) {
      Logger.Log("TooltipManager::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<TooltipManager>();
    }
    return _instance;
  }
  void Awake()
  {
    Logger.Log("TooltipManager::Awake", Logger.Level.DEBUG);
    _instance = this;
    loadDataIntoDico(inputFiles, _loadedInfoWindows);
  }
  ////////////////////////////////////////////////////////////////////////////////////////////


  public string[] inputFiles;

  private UISprite _backgroundSprite;
  private UILabel _titleLabel;
  private UILabel _typeLabel;
  private UILabel _subtitleLabel;
  private UISprite _illustrationSprite;
  private UILabel _customFieldLabel;
  private UILabel _customValueLabel;
  private UILabel _lengthValueLabel;
  private UILabel _referenceValueLabel;
  private UILabel _energyConsumptionValueLabel;
  private UILabel _explanationLabel;

  //public GameObject _tooltipPanel;
  private UIPanel _tooltipPanel;
  public TooltipPanel bioBrickTooltipPanel;
  public TooltipPanel deviceTooltipPanel;

  public string deviceBackground;
  public string bioBrickBackground;

  public static string getDeviceBackground()
  {
    return _instance.deviceBackground;
  }
  public static string getBioBrickBackground()
  {
    return _instance.bioBrickBackground;
  }

  public GameStateController gameStateController;

  private Dictionary<string, TooltipInfo> _loadedInfoWindows = new Dictionary<string, TooltipInfo>();
  private static string _bioBrickPrefix = "b_";
  private static string _devicePrefix = "d_";

  public enum Quadrant {
    TOP_LEFT,
    TOP_RIGHT,
    BOTTOM_LEFT,
    BOTTOM_RIGHT
  }

  public enum TooltipType {
    BIOBRICK,
    DEVICE
  }

  public Camera uiCamera;

  private static void setVarsFromTooltipPanel(TooltipType type)
  {
    TooltipPanel panel;

    switch(type)
    {
      case TooltipType.BIOBRICK:
        panel = _instance.bioBrickTooltipPanel;
        break;
      case TooltipType.DEVICE:
        panel = _instance.deviceTooltipPanel;
        break;
      default:
        panel = _instance.bioBrickTooltipPanel;
        break;
    }
    _instance._tooltipPanel = panel.gameObject.GetComponent<UIPanel>();

    _instance._titleLabel = panel.titleLabel;
    _instance._typeLabel = panel.typeLabel;
    _instance._subtitleLabel = panel.subtitleLabel;
    _instance._illustrationSprite = panel.illustrationSprite;
    _instance._customFieldLabel = panel.customFieldLabel;
    _instance._customValueLabel = panel.customValueLabel;
    _instance._lengthValueLabel = panel.lengthValueLabel;
    _instance._referenceValueLabel = panel.referenceValueLabel;
    _instance._energyConsumptionValueLabel = panel.energyConsumptionValueLabel;
    _instance._explanationLabel = panel.explanationLabel;
    _instance._backgroundSprite = panel.backgroundSprite;
  }
  //public float scale;

  /*
  private static IDictionary<BioBrick.Type, IDictionary<string, string>> BioBrickTextureNames = new Dictionary<BioBrick.Type, IDictionary<string, string>>()
  {
    {BioBrick.Type.PROMOTER, new Dictionary<string, string>(){
        {"PRCONS", "b_PRCONS"},
        {"PRLACI", "b_PRLACI"},
        {"PRTETR", "b_PRTETR"}
      }
    },
    {BioBrick.Type.RBS, new Dictionary<string, string>(){
        {"RBS1", "b_RBS1"},
        {"RBS2", "b_RBS2"}
      }
    },
    {BioBrick.Type.GENE, new Dictionary<string, string>(){
        {"FLUO1", "b_FLUO1"},
        //{"FLUO2", "gfp"},
        {"MOV", "b_MOV"},
        {"AMPR", "b_AMPR"}
        //{"REPR1", "repr1"},
        //{"REPR2", "repr2"}
      }
    },
    {BioBrick.Type.TERMINATOR, new Dictionary<string, string>(){
        {"DTER", "b_DTER"}
      }
    }
  };

  private static IDictionary<string, string> DeviceTextureNames = new Dictionary<string, string>()
  {
    //{"Ampicillin resistance (med)", "ampR_m"},
    {"Green fluorescence (low)", "d_cons-gfp-low"},
    {"Green fluorescence (med)", "d_cons-gfp-med"},
    {"Hyperflagellation (low)", "d_cons-speed-low"},
    {"Hyperflagellation (med)", "d_cons-speed-med"},
    //{"Regulated ampicillin resistance (med)", "ampR_m"},
    {"Regulated green fluorescence (low)", "d_plac-gfp-low"},
    {"Regulated green fluorescence (med)", "d_plac-gfp-med"},
    {"Regulated hyperflagellation (low)", "d_plac-speed-low"},
    {"Regulated hyperflagellation (med)", "d_plac-speed-low"}
  };
  */

  public static bool displayTooltip()
  {
    _instance._tooltipPanel.gameObject.SetActive(false);
    return true;
  }

  public static bool displayTooltip(bool isOver, Device device, Vector3 pos)
  {
    string code = (null == device)?null:_devicePrefix+device.getName();
    return displayTooltip(isOver, code, pos);
  }

  public static bool displayTooltip(bool isOver, BioBrick brick, Vector3 pos)
  {
    string code = (null == brick)?null:_bioBrickPrefix+brick.getName();
    return displayTooltip(isOver, code, pos);
  }

  private static bool displayTooltip(bool isOver, string code, Vector3 pos)
  {
    if(!isOver || (null == code))
    {
      return displayTooltip();
    }
    else
    {
      if(fillInFieldsFromCode(code))
      {
        _instance._tooltipPanel.gameObject.SetActive(true);

        //Rect rectout = _instance._tooltipPanel.GetAtlasSprite().outer;
        //_instance._tooltipPanel.gameObject.transform.localScale = new Vector3(_instance.scale*rectout.width, _instance.scale*rectout.height, 1f);

        setPosition(pos);
        return true;
      }
      else
      {
        Logger.Log("TooltipManager::displayTooltip("+code+") failed", Logger.Level.WARN);
        return false;
      }
    }
  }

  private static bool fillInFieldsFromCode(string code)
  {

    TooltipInfo info = retrieveFromDico(code);

    if(null != info)
    {
      setVarsFromTooltipPanel(info._tooltipType);

      _instance._backgroundSprite.spriteName   = info._background;
      _instance._titleLabel.text               = info._title;
      _instance._typeLabel.text                = info._type;
      _instance._subtitleLabel.text            = info._subtitle;

      if(null != _instance._illustrationSprite)
        _instance._illustrationSprite.spriteName = info._illustration;

      if((null != _instance._customFieldLabel) && (null != _instance._customValueLabel))
      {
        _instance._customFieldLabel.text = info._customField;
        _instance._customValueLabel.text = info._customValue;
      }

      _instance._lengthValueLabel.text              = info._length;
      _instance._referenceValueLabel.text           = info._reference;

      if(null != _instance._energyConsumptionValueLabel)
        _instance._energyConsumptionValueLabel.text   = info._energyConsumption;

      _instance._explanationLabel.text         = info._explanation;

      return true;
    }
    else
    {
      return false;
    }
  }

  private static TooltipInfo retrieveFromDico(string code)
  {
    TooltipInfo info;
    if(!_instance._loadedInfoWindows.TryGetValue(code, out info))
    {
      Logger.Log("TooltipManager::retrieveFromDico("+code+") failed", Logger.Level.WARN);
      info = null;
    }
    return info;
  }

  private void loadDataIntoDico(string[] inputFiles, Dictionary<string, TooltipInfo> dico)
  {

    TooltipLoader tLoader = new TooltipLoader();

    string loadedFiles = "";

    foreach (string file in inputFiles) {
      foreach (TooltipInfo info in tLoader.loadInfoFromFile(file)) {
        dico.Add(info._code, info);
      }
      if(!string.IsNullOrEmpty(loadedFiles)) {
        loadedFiles += ", ";
      }
      loadedFiles += file;
    }

    Logger.Log("TooltipManager::loadDataIntoDico loaded "+loadedFiles, Logger.Level.DEBUG);
  }

  private static void setPosition(Vector3 pos)
  {
    Quadrant quadrant = getQuadrant(pos);
    float xShift, yShift;

    float xScale = _instance._backgroundSprite.transform.localScale.x
      , yScale = _instance._backgroundSprite.transform.localScale.y;

    xShift = xScale/2;
    yShift = yScale/2;

    switch(quadrant)
    {
      case Quadrant.TOP_LEFT:
        yShift = -yShift;
        break;
      case Quadrant.TOP_RIGHT:
        xShift = -xShift;
        yShift = -yShift;
        break;
      case Quadrant.BOTTOM_LEFT:
        break;
      case Quadrant.BOTTOM_RIGHT:
        xShift = -xShift;
        break;
      default:
        Logger.Log("TooltipManager::setPosition default case", Logger.Level.WARN);
        break;
    }
    _instance._tooltipPanel.transform.position = new Vector3(pos.x, pos.y, pos.z);
    Vector3 localPos2 = _instance._tooltipPanel.transform.localPosition;
    _instance._tooltipPanel.transform.localPosition = new Vector3(localPos2.x + xShift, localPos2.y + yShift, -0.9f);
  }

  private static Quadrant getQuadrant(Vector3 pos)
  {
    Vector3 screenPos = _instance.uiCamera.WorldToScreenPoint(pos);
    bool top = screenPos.y > _instance.uiCamera.pixelHeight/2;
    bool left = screenPos.x < _instance.uiCamera.pixelWidth/2;

    if (top)
    {
      if (left)
        return Quadrant.TOP_LEFT;
      else
        return Quadrant.TOP_RIGHT;
    }
    else
    {
      if (left)
        return Quadrant.BOTTOM_LEFT;
      else
        return Quadrant.BOTTOM_RIGHT;
    }
  }

  void Start()
  {
    bioBrickTooltipPanel.gameObject.SetActive(false);
    deviceTooltipPanel.gameObject.SetActive(false);
  }
}