using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


/*!
  \brief This class describe a Molecule
  \details A molecule is define by :
        - A name -> Used for identification
        - A type (Enzyme, transcription factor or other)
        - A description (optionnal)
        - A concentration
        - A degradation rate -> used for degradation reaction
        - A Size -> used for fick reaction (not implemented yet)


Molecules wich are declared in files should respect this synthax :

      <molecule type="other">
        <name>H2O</name>
        <description>de l'eau!</description>
        <concentration>0</concentration>
        <degradationRate>0.013</degradationRate>
        <FickFactor>0.33</FickFactor>
      </molecule>


   \author Pierre COLLET
   \mail pierre.collet91@gmail.com
 */
public class Molecule
{
  //! Define Molecule type
  public enum eType
  {
    ENZYME,
    TRANSCRIPTION_FACTOR,
    OTHER
  }

  private string _name;                           //!< The use name of the molecule
  private string _realName = null;                //!< The real name of the molecule
  private eType _type;                            //!< The type of the molecule
  private string _description;                    //!< The description of the molecule (optionnal)
  private float _concentration;                   //!< The concentration of the molecule
  private float _newConcentration;                //!< The concentration of the molecule for the next stage
  private float _degradationRate;                 //!< The degradation rate of the molecule
  private float _fickFactor;                      //!< The FickFactor is a coefficient for FickReaction
  private float _negligibilityThreshold = 1E-10f; //!< The threshold below which the concentration is rounded down to 0.
	
  private bool _debug = false;
	
  //! Default constructor
  public Molecule(Molecule mol = null)
  {
    if (mol != null)
      {
		if(_debug) Logger.Log("Molecule::Molecule("+mol+")", Logger.Level.TRACE);
        _name = mol._name;
        _realName = GameplayNames.getMoleculeRealName(_name);
        _type = mol._type;
        _description = mol._description;
        _concentration = mol._concentration;
        _degradationRate = mol._degradationRate;
        _fickFactor = mol._fickFactor;
        _newConcentration = mol._newConcentration;
		if(_debug) Logger.Log("Molecule::Molecule("+mol+") built "+this, Logger.Level.TRACE);
      } else {
		if(_debug) Logger.Log("Molecule::Molecule(null)", Logger.Level.TRACE);
	  }
  }

  public string getName() {return _name; }
  public string getRealName() {return _realName; }
  public eType getType() {return _type; }
  public string getDescription() {return _description; }
  public float getConcentration() {
		if(_debug) Logger.Log ("Molecule::getConcentration "+_name+" "+_concentration, Logger.Level.TRACE);
		return _concentration;
  }
  public float getDegradationRate() {return _degradationRate; }
  public float getFickFactor() { return _fickFactor; }
  public void setName(string name) { _name = name; }
  public void setType(eType type) { _type = type; }
  public void setDescription(string description) { _description = description; }
  public void setConcentration(float concentration) {
	float oldConcentration = _concentration;
	_concentration = concentration; if (_concentration < _negligibilityThreshold) _concentration = 0;
	if(_debug) Logger.Log("Molecule::setConcentration("+concentration+") "+_name+" old="+oldConcentration+", new="+_concentration, Logger.Level.TRACE);
  }
  public void setDegradationRate(float degradationRate) { _degradationRate = degradationRate; }
  public void addNewConcentration(float concentration) {
	float oldNewCC = _newConcentration;
	_newConcentration += concentration; if (_newConcentration < _negligibilityThreshold) _newConcentration = 0;
	if(_debug) Logger.Log("Molecule::addNewConcentration("+concentration+") "+_name+" oldNewCC="+oldNewCC+", new="+_newConcentration, Logger.Level.TRACE);
  }
  public void subNewConcentration(float concentration) {
	float oldNewCC = _newConcentration;
	_newConcentration -= concentration; if (_newConcentration < _negligibilityThreshold) _newConcentration = 0;
	if(_debug) Logger.Log("Molecule::subNewConcentration("+concentration+") "+_name+" oldNewCC="+oldNewCC+", new="+_newConcentration, Logger.Level.TRACE);
  }
  public void setNewConcentration(float concentration) {
	float oldNewCC = _newConcentration;
	_newConcentration = concentration; if (_newConcentration < _negligibilityThreshold) _newConcentration = 0;
	if(_debug) Logger.Log("Molecule::setNewConcentration("+concentration+") "+_name+" old="+oldNewCC+", new="+_newConcentration, Logger.Level.TRACE);
  }
  public void setFickFactor(float v) { _fickFactor = v; }

  /*!
    \brief Add molecule concentration
    \param concentration The concentration
   */
  public void addConcentration(float concentration) {
	float oldCC = _concentration;
	_concentration += concentration; if (_concentration < _negligibilityThreshold) _concentration = 0;
	if(_debug) Logger.Log("Molecule::addConcentration("+concentration+") "+_name+" old="+oldCC+", new="+_concentration, Logger.Level.TRACE);
  }

  /*!
    \brief Add molecule concentration
    \param concentration The concentration
   */
  public void subConcentration(float concentration) {
	float oldCC = _concentration;
	_concentration -= concentration; if (_concentration < _negligibilityThreshold) _concentration = 0;
	if(_debug) Logger.Log("Molecule::subConcentration("+concentration+") "+_name+" old="+oldCC+", new="+_concentration, Logger.Level.TRACE);
  }

  //! \brief This function set the actual concentration to it new value
  public void updateConcentration() {
	if(_debug) Logger.Log("Molecule::updateConcentration() "+_name+" old="+_concentration+", new="+_newConcentration, Logger.Level.TRACE);
	_concentration = _newConcentration;
  }
	
  public override string ToString() {
    return "Molecule[name:"+_name
			+", t:"+_type
			+", d:"+_description
			+", cc:"+_concentration
			+", ncc:"+_newConcentration
			+", dr:"+_degradationRate
			+", ff:"+_fickFactor
			+"]";
  }
  public string ToShortString(bool displayAll) {
	if (!displayAll && _concentration == 0) {
	  return null;
	} else {
	  return _realName+":"+_concentration;
	}
  }
}


/*!
  \brief This classe describe a Product
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
public class Product
{
  protected string      _name;                  //! The name of the molecule
  protected float       _quantityFactor;        //! The factor of production

  public Product() { }
  public Product(string name, float quantityFactor) {
    _name = name;
	_quantityFactor = quantityFactor;
  }
  public Product(Product p)
  {
    _name = p._name;
    _quantityFactor = p._quantityFactor;
  }

  public void setName(string name) { _name = Tools.epurStr(name); }
  public string getName() { return _name; }
  public void setQuantityFactor(float quantity) { _quantityFactor = quantity; }
  public float getQuantityFactor() { return _quantityFactor; }

  public override bool Equals (object obj)
  {
    Product product = obj as Product;
    return (product != null)
      && _name           == product._name
      && _quantityFactor == product._quantityFactor;
  }

  public override string ToString() {
    return "Product[name:"+_name+", quantityFactor:"+_quantityFactor+"]";
  }
}

/*!
  \brief This class is an interface that each reaction has to inherit.
  \details It contains a name a list of products an a activation boolean.
  The react() function should be implemented in inheritant classes.
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
public abstract class IReaction
{
  protected string _name;                       //!< The name of the reaction
  protected LinkedList<Product> _products;      //!< The list of products
  protected bool _isActive;                     //!< Activation booleen
  protected Medium _medium;                     //!< The medium where the reaction will be executed
  protected float _reactionSpeed;               //!< Speed coefficient of the reaction
  protected float _energyCost;                  //!< Energy consumed by the reaction
  public bool enableSequential;
  public bool enableEnergy;

  //! Default constructor
  public IReaction()
  {
    _products = new LinkedList<Product>();
    _isActive = true;
    _reactionSpeed = 1f;
    _energyCost = 0f;

    //TODO CHECK THIS
    //enableSequential = true;
    //enableEnergy = false;
    enableSequential = ReactionEngine.get ().enableSequential;
    enableEnergy = ReactionEngine.get ().enableEnergy;
  }

  //! Copy Constructor
  public IReaction(IReaction r)
  {
    _products = new LinkedList<Product>();
    foreach(Product p in r._products)
      _products.AddLast(p);
      _isActive = r._isActive;
      _reactionSpeed = r._reactionSpeed;
      _energyCost = r._energyCost;
      enableSequential = r.enableSequential;
      enableEnergy = r.enableEnergy;
      _medium = r._medium;
  }

  public void setName(string name) { _name = Tools.epurStr(name); }
  public string getName() { return _name; }
  public void setReactionSpeed(float speed) { _reactionSpeed = speed; }
  public float getReactionSpeed() { return _reactionSpeed; }
  public float getEnergyCost() { return _reactionSpeed; }
  public void setEnergyCost(float energy) { _energyCost = energy; }
  public Medium getMedium() { return _medium; }
  public void setMedium(Medium med) { _medium = med; }


  /*!
    \brief This function copies a reaction by calling its real copy constructor
    (not IReaction constructor but for example PromoterReaction constructor)
    \param r Reaction to copy
    \return Return a reference on the new reaction or null if the given reaction is unknown.
   */
  public static IReaction       copyReaction(IReaction r)
  {
    if (r as PromoterReaction != null)
      return new PromoterReaction(r as PromoterReaction);
    if (r as Allostery != null)
      return new Allostery(r as Allostery);
    if (r as InstantReaction != null)
      return new InstantReaction(r as InstantReaction);
    if (r as EnzymeReaction != null)
      return new EnzymeReaction(r as EnzymeReaction);
    if (r as ActiveTransportReaction != null)
      return new ActiveTransportReaction(r as ActiveTransportReaction);
    if (r as FickReaction != null)
      return new FickReaction(r as FickReaction);
    if (r as ATPProducer != null)
      return new ATPProducer(r as ATPProducer);
    if (r as Degradation != null)
      return new Degradation(r as Degradation);
    return null;
  }

  //! This function should be implemented by each reaction that inherit from this class.
  //! It's called at each tick of the game.
  public abstract void react(ArrayList molecules);

  /*! 
    \brief Add a Product to the product list.
    \param prod The product to be added to the list
   */
  public void addProduct(Product prod) { if (prod != null) _products.AddLast(prod); }
	
  public override string ToString ()
	{
		return string.Format ("IReaction[name:{0}, products:{1}, isActive:{2}, medium:{3}, "
			+"reactionSpeed:{4}, energyCost:{5}, enableSequential:{6}, enableEnergy:{7} ]",
		  _name,                                    //!< The name of the reaction
          Logger.ToString<Product>(_products),      //!< The list of products
          _isActive,                                //!< Activation booleen
          _medium,                                  //!< The medium where the reaction will be executed
          _reactionSpeed,                           //!< Speed coefficient of the reaction
          _energyCost,                              //!< Energy consumed by the reaction
          enableSequential,
          enableEnergy
		);
	}


  /* !
    \brief Checks that two reactions are equal.
    \param reaction The reaction that will be compared to 'this'.
    \param nameMustMatch Whether the name must be taken into account or not.
   */
  public bool Equals(IReaction reaction, bool checkNameAndMedium)
  {
    if(checkNameAndMedium)
    {
      return Equals (reaction);
    }
    return BaseCharacEquals(reaction) && CharacEquals(reaction);
  }


  /* !
    \brief Checks that two reactions have the same IReaction field values.
    \param reaction The reaction that will be compared to 'this'.
   */
  public bool BaseCharacEquals(IReaction reaction)
  {
    bool res =
         //_products.Equals(reaction._products)
         LinkedListExtensions.Equals(_products,reaction._products)
      && (_isActive        == reaction._isActive)
      //&& _medium.Equals(reaction._medium)
      && (_reactionSpeed   == reaction._reactionSpeed)
      && (_energyCost      == reaction._energyCost)
      && (enableSequential == reaction.enableSequential)
      && (enableEnergy     == reaction.enableEnergy)
      ;

    /*
      //bool productivity =   _products.Equals(reaction._products);
      bool productivity = LinkedListExtensions.Equals<Product>(_products,reaction._products);
      bool activity = (_isActive        == reaction._isActive);
      //&& _medium.Equals(reaction._medium)
      bool reactivity = (_reactionSpeed   == reaction._reactionSpeed);
      bool costity = (_energyCost      == reaction._energyCost);
      bool sequentiality = (enableSequential == reaction.enableSequential);
      bool energetity = (enableEnergy     == reaction.enableEnergy);

    Logger.Log ("IReaction::BaseCharacEquals "
      +", productivity ="+productivity
      +", activity ="+activity
      +", reactivity ="+reactivity
      +", costity ="+costity
      +", sequentiality ="+sequentiality
      +", energetity ="+energetity
      , Logger.Level.TRACE);
    */

    return res;
  }

  /* !
    \brief Checks that two reactions have the same child class field values.
    \param reaction The reaction that will be compared to 'this'.
   */
  protected abstract bool CharacEquals(IReaction reaction);
}

// ========================== DEGRADATION ================================

/*!
  \brief This class manages degradation reactions
  \details
  Manage the degradation of a specific molecule inside the cell or in a specific medium.
  In this simulation, the degradation is determined by the degradation constant (half-life) of a specific chemical or protein.
  \author Pierre COLLET
  \mail pierre.collet91@gmail.com
 */
public class Degradation : IReaction
{
  private float _degradationRate;               //! The degradation speed
  private string _molName;                      //! Molecule name's

  //! Default constructor
  public Degradation(float degradationRate, string molName)
  {
    _degradationRate = degradationRate;
    _molName = molName;
  }

  //! Copy Constructor
  public Degradation(Degradation r) : base(r)
  {
    _degradationRate = r._degradationRate;
    _molName = r._molName;
  }

  /* !
    \brief Checks that two reactions have the same Degradation field values.
    \param reaction The reaction that will be compared to 'this'.
   */
  protected override bool CharacEquals(IReaction reaction)
  {
    Degradation degradation = reaction as Degradation;
    return (degradation != null)
    && (_degradationRate == degradation._degradationRate)
    && (_molName == degradation._molName);
  }

  /*!
    \details The degradation reaction following the formula above:

                [X] = degradationRate * [X]

    \param molecules The list of molecules
   */
  public override void react(ArrayList molecules)
  {
    if (!_isActive)
      return;

    Molecule mol = ReactionEngine.getMoleculeFromName(_molName, molecules);
    float delta = mol.getDegradationRate() * mol.getConcentration();
    if (enableSequential)
      mol.subConcentration(mol.getDegradationRate() * mol.getConcentration() * _reactionSpeed * ReactionEngine.reactionsSpeed);
    else
      mol.subNewConcentration(delta * _reactionSpeed * ReactionEngine.reactionsSpeed);
  }
}