using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BM_Weapon_System_NewC : MonoBehaviour {

	[Tooltip("This is a Beta Release of a C# BattleMancer Weapon Systems. If you encounter any bugs or strange behaviour of any of the Systems, please, contact the Author at Marcin.G.Tomala@Gmail.com , so they can be fixed as fast as possible!  [This is an INFO variable. It does nothing, it just holds the Implemented documentation mouse-over tooltip with info. :) ]")]
	public string BattlemancerBetaNotes = "<Mouse-over for Beta Notes>";
	
	[Header("Overall Settings")]
		
		

		[Tooltip("Can this weapon be controlled with Player Input (keyboard, mouse, gamepad, etc.)? If False, weapon can only be shot remotely by calling  StartCoroutine (or SendMessaging) Fire_Primary() and Fire_Secondary().")]	 
			public bool CanControl = true;
	
	
	[Tooltip("What's the object, that has an Animation Component with all of the weapon's animations? In later variables, this object is reffered to simply as Animations Object.")]
		public Animator animator;
	
	[Tooltip("[INFO:] If you want to use any of the Audio Features, please attach the AudioSource to this GameObject. [This is an Info Variable. It does nothing - it's here for the tooltip of the Implemented Documentation]")]
	public string AudioInfo = "<Mouse-over for info>";
	
	[Tooltip("How many seconds have to pass from instantiation (initialization/spawning) of a Weapon before the first shot can be fired? [HINT:] Useful if you don't want the Player to fire a weapon right after he switches to it, preventing any fire-spamming exploits (namely The Quick Switch Shooting).")] 
		public float InitialManualReloadTime  = 0.4f;
	
	[Tooltip("Does this weapon generate a sound when initiated? (Like the sound of a sword being drawn from it's sheath)")]
		public AudioClip InitialSound ;
	
	[Tooltip("Does this weapon have an Idle animation (played when nothing happens)? If yes, make sure that the Animations Object has an animation called Idle attached!")]
		public bool HasIdleAnimations ;
	
	[Tooltip("The Inspect Animation is an optional animation of a weapon which has purely cosmetic value (like checking your gun, playing with a knife or taunting). The inspect animation file should be named Inspect and be attached to the Animation Component of the Animations Object.  Additionally, an action called Inspect should be set in the Input Manager.")]
		public bool HasInspectAnimation ;
	
	[Tooltip("If there is an Inspect Animation, does it have a sound? If yes, what is the Audio Clip for it?")]
	public AudioClip InspectSound ; 
	
	private bool  inspecting ;
	private Vector2 DamagePack  ;
	
	[Space(30)]
		[Header("Primary Fire Features")]
			
			[Tooltip("Does this weapon have Primary Fire?")]
			public bool HasPrimaryFire ; 
	
	[Tooltip("What's the name of the Input Menager action for primary fire?")]
	 public  string PrimaryFireInput  = "Fire";
	
	[Space (10) ]
		
		[Tooltip("Is this weapon Primary fire mode Semi-Automatic (each attack requires another pull of the trigger) or Fully-Automatic (hold trigger to fire constantly)? Check True if Semi, leave False if Automatic.")]
			public bool PrimarySemi ;
	
	[Tooltip("Does this weapon use PrimaryAmmo?")]
		public bool UsesPrimaryAmmo  = false;
	
	[Tooltip("Check this variable True if this weapon's SpareAmmo source should never go below it's current value. [HINT:] If you want to force the current Ammo (in the clip) to never end, just set this weapon to not UsePrimaryAmmo.")]
	public bool  InfinitePrimaryMaxAmmo ;
	
	[Tooltip("Does this weapon use reloading (for example Rifle magazines - check True), or does it have a single ammo source (like a Flamer fuel tank - check False)? Make sure that the Input Menager has a defined action called Reload Primary.")] 
	public bool  PrimaryReloads ;
	
	[Tooltip("Should the primary weapon be automaticaly reloaded when attacking without ammo in the clip, or should it only be manually reloaded?")]
	public bool  AutoReloadPrimary ;
	
	[Tooltip("Does this weaon's Primary Reloading have an animation? If True, make sure that the Animation Object has an animation called Reload_Primary. This weapon will not fire again until the Reload Animation is complete.")]
	public bool  PrimaryHasReloadAnimation ;
	
	[Tooltip("After how many seconds the Reload will be finished? (Counted from the end of the Reload Animation, if weapon has it, or from pressing Reload Primary input otherwise.)")]
	public float PrimaryManualReloadTime ;
	
	[Tooltip("If Reloading has a sound, place it's AudioClip here!")]
	 public AudioClip PrimaryReloadSound ;
	
	[Tooltip ("The starting Amunition amount (if Reoads is checked True), or the amount of the PrimaryAmmo Supply (if False).")]
	 public int  PrimaryAmmo ;
	
	[Tooltip("How much PrimaryAmmo does each shot take?")]
	 public int  PrimaryAmmoPerShot  = 1;
	
	[Tooltip("If the weapon doesn't have enough Ammo to pass the Ammo Per Shot test, but it's still not zero, should it fire anyway and drain it's ammo clip to zero? Otherwise, to fire this weapon you will need at least PrimaryAmmoPerShot of spare Ammo to fire. (Live example: A gun firing a big blob of goo which takes 30 ammo per shot - should it fire anyway if it has less than 30 ammo?)")]
	public  bool PrimaryAllowIncompleteLastShot ;
	
	[Tooltip("How many bullets (shots) does one clip of this weapon hold?")]
	 public int  PrimaryAmmoClipSize ;
	
	[Tooltip("If this weapon must be reloaded, how much Spare Ammo does it have right now?")]
	 public int  SparePrimaryAmmo ;
	
	[Tooltip("What is the max limit of Spare Ammo (if this weapon reloads) or simply Ammo (if it doesn't reload) that this weapon can carry?")]
	 public  int MaxPrimaryAmmo ;
	
	[Tooltip("Does the weapon have a sound for dry fire (when PrimaryAmmo is zero)? If yes, put it's audioclip here.")]
	 public AudioClip  PrimaryDryFireSound ;
	
	private  bool Reloading ;
	private   bool DryFireReady  = true;
	
	[Space (10)]
		
		[Tooltip("What's the interval (how many seconds) between each shot?")]
	 public float PrimaryFireInterval  = 0.5f;
	
	[Tooltip("What's the manual delay (in seconds, independent from Warm-Up Animation delay), before the attack is initiated?")]
	 public float PrimaryManualFireDelay  = 0f;
	
	[Tooltip("After how many seconds from the starting of attack animation should the attack be fired? In other words: On an attack, your weapon will start it's animation, but any projectiles or raycasts will be fired after this amount of time. Useful for synchronizing attack animations with attack's effects (for example, making a raycast fired in the exact time the dagger's animation is at it's peak). [INFO:] Does stack with Manual Fire Delay.")] 
	 public float PrimaryShotDelay ;
	
	[Tooltip("How many bullets (pellets in a shotgun blast, for example) does this weapon fire? Works both on Raycasts and Projectiles.")]
	 public int  PrimaryPelletsPerShot  = 1;
	
	[Tooltip("What's the vertical inaccuracy of the weapon? Works both on Projectiles and Raycasts, also if multiple are fired at once (Shotgun effect). [HINT:] Variable's scale differs depending on weapon type. For comparison, in Raycast, a typical pistol inacuracy is about 0.02-0.05. In Projectile pistol, similar results can be achieved at about 1-2.")]
	 public float PrimaryHorizontalBulletSpread  = 0;
	
	[Tooltip("What's the horizontal inaccuracy of the weapon? Works both on Projectiles and Raycasts, also if multiple are fired at once (Shotgun effect). [HINT:] Variable's scale differs depending on weapon type. For comparison, in Raycast, a typical pistol inacuracy is about 0.02-0.05. In Projectile pistol, similar results can be achieved at about 1-2.")]
	 public float PrimaryVerticalBulletSpread  = 0;
	
	
	
	[Space(10)]
		
		
		[Tooltip ("A Weapon can have a special animation played before the attack starts (for example a Minigun wind-up or Spell Casting animation). If True, the Animation Object should have an animation file called WarmUp_Primary attached. IMPORTANT: This also automatically delays an attack by the length (duration) of the WarmUp animation itself, independent from Manual Delay time.")]
	 public bool PrimaryHasWarmUpAnimation ;
	
	[Tooltip("If the above is True, what's the sound clip for the WarmUp animation? (like whizz of the Minigun spinning engine)")]
	 public AudioClip PrimaryWarmUpSound ;
	
	[Tooltip ("A Weapon can have a cosmetic cooldown animation played after the attack is done (like putting down a wand after a long burst of Magic Missiles, or a Minigun slowing down after constant firing). If True, the Animation Object should have an animation file called Cooldown_Primary attached.")]
	 public bool PrimaryHasCooldownAnimation ;
	
	[Tooltip ("After how many seconds the cooldown animation should kick in after firing?")]
	 public float PrimaryCooldownDelay ;
	
	[Tooltip("Is there a sound clip for the Cooldown animation? (like the spin-down of the Minigun engine or energy dispersing after spell casting).")]
	 public AudioClip PrimaryCooldownSound ;
	
	[Tooltip("Weapon can have an animation playing after each shot (shotgun pump, bolt-action of a rifle, etc.). The interval animation file must be named Interval_Primary and be assigned to the Animations Object. Checking this option True will automatically delay all other actions by the length (duration) of the Interval animation (so you won't fire another shot until it's done).")]
	 public bool PrimaryHasIntervalAnimation ;
	
	[Tooltip("If the above is True, what's the sound clip for the interval animation? (like the sound of a shotgun pump)")]
	 public AudioClip PrimaryIntervalSound ;
	
	[Space(10)]
		
		[Tooltip("How many Animations does the Animations Object have for the Primary Fire? A weapon can have from 1 to 4 attack animations. By default, the first animation file must be named Fire_Primary1, and each next: Fire_Primary2, Fire_Primary3 and Fire_Primary4. 0 means there are no animations.")]
	 public int  MaxPrimaryAttackAnimations ;
	
	[Space(10)]
		
		[Tooltip("An attack can emit a continous sound (like a flamethrower), while the Attack Input is being pressed. If you wish to use this sound, put it's clip here. [IMPORTANT!] Using Continous sound clip with Fire Sound (below) might cause someunforseen consequences. It's not adviced to use both of these options at one. [IMPORTANT 2!] This option will not work with Semi firing mode (so only automatic weapons can use it).")]
	 public AudioClip PrimaryConstantSound ;
	
	[Tooltip("How many sound clips does the Primary Fire have? (set from 0 to 5 and assign sounds in the fields below)")]
	 public int  MaxPrimaryFireSounds ;
	 public AudioClip PrimaryFireSound1 ;
	 public AudioClip PrimaryFireSound2 ;
	 public AudioClip PrimaryFireSound3 ;
	 public  AudioClip PrimaryFireSound4 ;
	 public AudioClip  PrimaryFireSound5 ;
	
	
	
	[Tooltip("How many sound clips does the Primary Fire Enviroment Hit have to randomize from? It's used by Raycast and Damage Area attacks - this sound is played, when they hit anything else than a Target (f.e. a wall). (Set from 0 to 5 and assign sounds in the fields below)")]
	 public int  MaxPrimaryEnviromentHitSounds ;
	 public AudioClip PrimaryEnviromentHitSound1 ;
	 public AudioClip PrimaryEnviromentHitSound2 ;
	 public AudioClip PrimaryEnviromentHitSound3 ;
	 public AudioClip PrimaryEnviromentHitSound4 ;
	 public  AudioClip PrimaryEnviromentHitSound5 ;
	
	
	[Tooltip("How many sound clips does the Primary Fire Target Hit have to randomize from? It's used by Raycast and Damage Area attacks - this sound is played, when they hit a Target. (Set from 0 to 5 and assign sounds in the fields below)")]
	 public int  MaxPrimaryTargetHitSounds ;
	 public AudioClip PrimaryTargetHitSound1 ;
	 public AudioClip PrimaryTargetHitSound2 ;
	 public AudioClip PrimaryTargetHitSound3 ;
	 public AudioClip PrimaryTargetHitSound4 ;
	 public AudioClip PrimaryTargetHitSound5 ;
	
	[Space (10)]
		
		[Tooltip("Where will the muzzle flash (fire from barrel, smoke, magical sparks, etc.) appear, if the weapon has it? Create an empty GameObject, make it a child of this weapon's Prefab and place it around your weapon in the desired position. If weapon doesn't have any muzzle flash (like an axe or a silenced pistol), leave this field empty")]
	 public Transform PrimaryFlashSourcePoint ; 
	
	[Tooltip("The muzzle flash to show after fire (a GameObject or Prefab) if weapon has it. If weapon doesn't have any muzzle flash (like an axe or a silenced pistol), leave this field empty")]
	 public GameObject  PrimaryMuzzleFlash ;
	
	[Space (10)]
		
		[Tooltip("Does the primary fire eject any GameObjects upon firing? (like empty cases and shells)")]
	 public bool PrimaryEjectsShells ;
	
	[Tooltip("From where will they be spawned? Create an empty GameObject, make it a child of this weapon's Prefab and place it around your weapon in the desired position. The shells are spawned according to z (blue) axis.")]
	 public Transform  PrimaryEjectedShellsSourcePoint ;
	
	[Tooltip("Prefab used as spawned shells.")]
	 public GameObject  PrimaryEjectedShells ;
	
	[Tooltip("Should the shells be propelled from start? If not, leave at zero. If they should, attach a rigidbody to the shell's GameObject!")]
	 public float PrimaryShellEjectionForce ;	
	
	
	[Space (20)]
		
		[Tooltip("Does this weapon fire Raycasts (a.k.a. Hitscan weapon - instant hiting, like ordinary firearms)?")]
	 public bool PrimaryFiresRaycast ;
	
	[Tooltip("What Layers should this weapon hit and which should be ignored (shot through) in Raycast (hitscan) Attacks? [HINT:]  Make sure it doesn't hit the Layer in which the Weapon model is (so you won't hit your weapon right after firing) and the Layer in which the Player is (so you won't hit yourself)!")]
	 public  LayerMask PrimaryRayTargetLayers  ;
	
	[Tooltip("What's the Tag of the Objects which can recieve Damage (and which use Blood)? [HINT:] Like Enemy or Weapon_Target.")]
	 public string PrimaryTargetTag  = "Enemy";
	
	[Tooltip("If it fires Raycasts, from where will they come from? Create an empty GameObject, make it a child of your Weapon Prefab and place it in a desired place around your weapon to specify this point - the attack will go along the Blue (z) axis.")]
	 public Transform  PrimaryRayBarrel ;
	
	[Tooltip("The GameObject or Prefab which appears, when Raycast hits an Object marked with Target Tag. (for example a blood splatter, robot parts flying, a burst of alien goo, etc.)")]
	 public GameObject  PrimaryBloodSplat ;
	
	[Tooltip("The GameObject or Prefab which appears, when Raycast hits something not signed with a weapon target Tag - for example a wall. (Bullet decals, sparks, splinters, etc)")]
	 public GameObject  PrimarySparks ;
	
	[Tooltip("If it fires Raycast, how much Damage will it deal? [ADVANCED:] Damage is sent to ApplyDamage()  void in the BM Health System script, as a Vector2(PrimaryRayDamage,PrimaryDamageType) called DamagePack.")]
	 public float PrimaryRayDamage ;
	
	[Tooltip("[OPTIONAL] What type of damage will it deal? Damage type is specified with a number and it is used for Resistances against various Damage Types in the BM Health System script. Changing this variable is not mandatory.")]
	 public int  PrimaryDamageType ;
	
	[Tooltip("If it fires Raycast, what's the maximum range of the weapon? If it doesn't matter, leave a high number, like 99999. Must be greater than zero!")]
	 public float PrimaryRayRange  = 100000;
	
	[Tooltip("Do shots apply pushing or pulling force on Rigidbodies?")]
	 public bool PrimaryRayAppliesForce ;
	
	[Tooltip("If it applies force, how strong will it be?")]
	 public float PrimaryRayForce ;
	
	[Space (5)]
		
		[Tooltip("Does this weapon create a sound in hit point, when Enviroment was hit with a raycast? (Like a sound of a ricocheting bullet)")]
	 public bool PrimaryRayHasEnviromentHitSounds ;
	
	[Tooltip("Does this weapon create a sound in hit point, when Target was hit with a raycast? (Like a sound of a robot parts being hit by a bullet or a blood splatter sound)")]	
	 public bool PrimaryRayHasTargetHitSounds ;
	
	
	
	
	[Space (20)]
		[Tooltip("Does this weapon fire a projectile or a damage area? (a fireball, a crossbow bolt, or a sword slash damage area, etc.)")]
	 public bool PrimaryFiresProjectile ;
	
	[Tooltip("If it fires Projectiles, from where will they come from? Create an empty GameObject, make it a child of your Weapon Prefab and place it in a desired place around your weapon to specify this point - the Projectile will spawn along the Blue (z) axis. [HINT:] For normal Projectiles, set it around the barrel of your weapon. For Melee Damage Areas set it in the middle of the space where Damage Area will have it's center (like around the center of a sword's blade in the highest point of it's attack animation).")]
	 public Transform  PrimaryProjectileBarrel ;
	
	[Tooltip("Should a fired Projectile be free, or should it stay relative to the Projectile Barrel position? [HINT:]  A ranged weapon, like a Crossbow or a Rocket Launcher should have this checked False, because their projectiles move. A Sword Slash Damage Area or a Chainsaw Damage Area will have this checked True, so they stay in front of the weapon.")] 
	 public bool StaticPrimaryProjectile  = false;
	
	[Tooltip("So... what's the Object or a Prefab used as a Projectile? [HINT:]  An object with BM Projectile System or BM Damage Area is advised, though not obligatory.")] 
	 public GameObject  PrimaryProjectile ;
	
	
	[Space (20)]
		
		[Tooltip("Does this weapon's Primary Fire create Damage Area upon firing? (Action RPG melee weapons, explosions, AoE spells etc.)")]
	 public bool PrimaryFiresDamageArea ;
	[Tooltip("From where will the Attack be cast? Create an empty GameObject, make it a child of this weapon and place it in desired place to specify this point. All attacks will come from this point and all Angle calculations will be done using it's Z axis (blue) as front. If this field is left empty, Attack Origin will be this object's center.")]
	 public Transform  PrimaryDAAttackOrigin ;
	[Tooltip("What Layers of Objects should be affected by this Damage Area's damage and/or push? (like Targets, clutter, explosive barrels, destructible enviroment, wooden crates etc.)")]
	 public  LayerMask PrimaryDADamageLayers  ;
	[Tooltip("What is the range of this Damage Area?")]
	 public float PrimaryDARange ;
	[Tooltip("What is the Angle, in which Targets will be hit? [INFO:] The Angle is calculated from the Attack Origin center, through it's Z axis (blue, as front), to the Target's center. It's highly adviced to add some margin of error!")]
	 public float PrimaryDAAngle ;
	[Tooltip("What is the Damage of this Damage Area? [ADVANCED:] Damage is sent to ApplyDamage()  void in the BM Health System script, as a Vector2(SecondaryRayDamage,SecondaryDamageType) called DamagePack.")]
	 public float PrimaryDADamage ;
	[Tooltip("[OPTIONAL] What type of damage will it deal? Damage type is specified with a number and it is used for Resistances against various Damage Types in the BM Health System script. Changing this variable is not mandatory.")]
	 public float PrimaryDADamageType ;
	[Tooltip("Should this Damage Area push or pull objects on Target Layers? The force is applied from the Attack Origin position.")] 
	 public bool PrimaryDAAppliesForce ;
	[Tooltip("If the above is True, how hard it does it push?")] 
	 public float PrimaryDAForce ;
	[Tooltip("[BETA!] Should this Damage Area take obstacles into account? Targets obstructed by obstacles will not be hit.")]
	 public bool PrimaryPerformDAObstacleCheck ;
	[Tooltip("What Layers should be taken into consideration while Obstacle Checking? (That is: Targets and obstacles. For example the Enemies, enviroment, force fields, etc.)." )]
	 public LayerMask  PrimaryDAObstacleCheckLayers  ;
	[Tooltip("What object should be spawned upon hitting a Target? If none, leave empty.")]
	 public GameObject  PrimaryDABlood ;
	[Tooltip("What object should be spawned upon hitting an object which is not a Target (like a wall)? If none, leave empty.")]
	 public GameObject  PrimaryDAMissSparks ; 
	[Tooltip("Should this Damage Area simulate hitting enviroment? It's an additional Raycast (hitscan), going out of the front of the Attack Origin, spawning sparks on hit.")]
	 public bool PrimaryDASimulateEnviromentHit ;
	[Tooltip("What Layers can be hit by the above simuation?")]
	 public  LayerMask PrimaryDASimEnviroment  ;
	[Tooltip("Should this Primary DA use Primary Enviroment Hit Sounds?")]
	 public bool PrimaryDAHasEviromentHitSounds ;
	[Tooltip("Should this Primary DA use Primary Target Hit Sounds?")]
	 public bool PrimaryDAHasTargetHitSounds ;
	
	 private List<GameObject> PrimaryAllTargets = new List<GameObject>();
	 private List<GameObject> PrimaryTargets = new List<GameObject>();
	
	
	
	[Space(30)]
		[Header("Secondary Fire Features")]
			
			
			[Tooltip("Does this weapon have Secondary Fire?")]
	 public bool HasSecondaryFire ; 
	
	[Tooltip("What's the name of the Input Menager action for Secondary fire?")]
	 public string SecondaryFireInput  = "Secondary Fire";
	
	[Space (10) ]
		
		[Tooltip("Is this weapon Secondary fire mode Semi-Automatic (each attack requires another pull of the trigger) or Fully-Automatic (hold trigger to fire constantly)? Check True if Semi, leave False if Automatic.")]
	 public bool SecondarySemi ;
	
	[Tooltip("Does this weapon use SecondaryAmmo? [CAUTION:] Do NOT use in combination with the variable below - the SecondaryUsesPrimaryAmmo.")]
	 public bool UsesSecondaryAmmo  = false;
	
	[Tooltip("Does this Secondary Fire use Primary Ammo Source as amunition? (For example, if a weapon uses the same ammo for both fire types, like a dual-barreled shotgun firing from one barrel or both at once). If True, PrimaryAmmo will be consumed while firing Secondary mode. [IMPORTANT!] Setting PrimaryAmmo as source for Secondary Fire does NOT support Secondary Reloading options. Simply put: if this is True, you can set only the SecondaryAmmoPerShot and AllowIncompleteShot variables.")] 
	 public bool SecondaryUsesPrimaryAmmo ;
	
	[Tooltip("Check this variable True if this weapon's SpareAmmo source should never go below it's current value. [HINT:] If you want to force the current Ammo (in the clip) to never end, just set this weapon to not UseSecondaryAmmo.")]
	 public bool InfiniteSecondaryMaxAmmo ;
	
	[Tooltip("Does this weapon use reloading (for example Rifle magazines - check True), or does it have a single ammo source (like a Flamer fuel tank - check False)? Make sure that the Input Menager has a defined action called Reload Secondary.")] 
	 public bool SecondaryReloads ;
	
	[Tooltip("Should the secondary weapon be automaticaly reloaded when attacking without ammo in the clip, or should it only be manually reloaded?")]
	 public bool AutoReloadSecondary ;
	
	[Tooltip("Does this weapon's Secondary Reloading have an animation? If True, make sure that the Animation Object has an animation called Reload_Secondary. This weapon will not fire again until the Reload Animation is complete.")]
	 public  bool SecondaryHasReloadAnimation ;
	
	[Tooltip("After how many seconds the Reload will be finished? (Counted from the end of the Reload Animation, if weapon has it, or from pressing Reload Secondary input otherwise.)")]
	 public float SecondaryManualReloadTime ;
	
	[Tooltip("If Reloading has a sound, place it's AudioClip here!")]
	 public AudioClip SecondaryReloadSound ;
	
	[Tooltip ("The starting Amunition amount (if Reoads is checked True), or the amount of the SecondaryAmmo Supply (if False).")]
	 public int  SecondaryAmmo ;
	
	[Tooltip("How much SecondaryAmmo does each shot take?")]
	 public int  SecondaryAmmoPerShot  = 1;
	
	[Tooltip("If the weapon doesn't have enough Ammo to pass the Ammo Per Shot test, but it's still not zero, should it fire anyway and drain it's ammo clip to zero? Otherwise, to fire this weapon you will need at least SecondaryAmmoPerShot of spare Ammo to fire. (Live example: A gun firing a big blob of goo which takes 30 ammo per shot - should it fire anyway if it has less than 30 ammo?)")]
	 public bool SecondaryAllowIncompleteLastShot ;
	
	[Tooltip("How many bullets (shots) does one clip of this weapon hold?")]
	 public int  SecondaryAmmoClipSize ;
	
	[Tooltip("If this weapon must be reloaded, how much Spare Ammo does it have right now?")]
	 public  int SpareSecondaryAmmo ;
	
	[Tooltip("What is the max limit of Spare Ammo (if this weapon reloads) or simply Ammo (if it doesn't reload) that this weapon can carry?")]
	 public int  MaxSecondaryAmmo ;
	
	[Tooltip("Does the weapon have a sound for dry fire (when SecondaryAmmo is zero)? If yes, put it's audioclip here.")]
	 public AudioClip SecondaryDryFireSound ;
	
	
	[Space (10)]
		
		[Tooltip("What's the interval (how many seconds) between each shot?")]
	 public float SecondaryFireInterval  = 0.5f;
	
	[Tooltip("What's the manual delay (in seconds, independent from Warm-Up Animation delay), before the attack is initiated?")]
	 public  float SecondaryManualFireDelay  = 0;
	
	[Tooltip("After how many seconds from the starting of attack animation should the attack be fired? In other words: On an attack, your weapon will start it's animation, but any projectiles or raycasts will be fired after this amount of time. Useful for synchronizing attack animations with attack's effects (for example, making a raycast fired in the exact time the dagger's animation is at it's peak). [INFO:] Does stack with Manual Fire Delay.")] 
	 public float  SecondaryShotDelay ;
	
	[Tooltip("How many bullets (pellets in a shotgun blast, for example) does this weapon fire? Works both on Raycasts and Projectiles.")]
	 public int  SecondaryPelletsPerShot  = 1;
	
	[Tooltip("What's the vertical inaccuracy of the weapon? Works both on Projectiles and Raycasts, also if multiple are fired at once (Shotgun effect). [HINT:] Variable's scale differs depending on weapon type. For comparison, in Raycast, a typical pistol inacuracy is about 0.02-0.05. In Projectile pistol, similar results can be achieved at about 1-2.")]
	 public float SecondaryHorizontalBulletSpread  = 0;
	
	[Tooltip("What's the horizontal inaccuracy of the weapon? Works both on Projectiles and Raycasts, also if multiple are fired at once (Shotgun effect). [HINT:] Variable's scale differs depending on weapon type. For comparison, in Raycast, a typical pistol inacuracy is about 0.02-0.05. In Projectile pistol, similar results can be achieved at about 1-2.")]
	 public float SecondaryVerticalBulletSpread  = 0;
	
	
	
	[Space(10)]
		
		
		[Tooltip ("A Weapon can have a special animation played before the attack starts (for example a Minigun wind-up or Spell Casting animation). If True, the Animation Object should have an animation file called WarmUp_Secondary attached. IMPORTANT: This also automatically delays an attack by the length (duration) of the WarmUp animation itself, independent from Manual Delay time.")]
	 public bool SecondaryHasWarmUpAnimation ;
	
	
	[Tooltip("If the above is True, what's the sound clip for the WarmUp animation? (like whizz of the Minigun spinning engine)")]
	 public AudioClip SecondaryWarmUpSound ;
	
	[Tooltip ("A Weapon can have a cosmetic cooldown animation played after the attack is done (like putting down a wand after a long burst of Magic Missiles, or a Minigun slowing down after constant firing). If True, the Animation Object should have an animation file called Cooldown_Secondary attached.")]
	 public bool SecondaryHasCooldownAnimation ;
	
	[Tooltip("If the above is True, Secondary fire can use the Primary cooldown Animation instead of a secondary one. Check this True if Primary and Secondary fire have the same cooldown animations.")]
	 public bool SecondaryHasPrimaryCooldown ;
	
	[Tooltip ("After how many seconds the cooldown animation should kick in after firing?")]
	 public float SecondaryCooldownDelay ;
	
	[Tooltip("Is there a sound clip for the WarmUp animation? (like the spin-down of the Minigun engine or energy dispersing after spell casting).")]
	 public AudioClip SecondaryCooldownSound ;
	
	[Tooltip("Weapon can have an animation playing after each shot (shotgun pump, bolt-action of a rifle, etc.). The interval animation file must be named Interval_Secondary and be assigned to the Animations Object. Checking this option True will automatically delay all other actions by the length (duration) of the Interval animation (so you won't fire another shot until it's done).")]
	 public bool SecondaryHasIntervalAnimation ;
	
	[Tooltip("If the above is True, what's the sound clip for the interval animation? (like the sound of a shotgun pump)")]
	 public AudioClip SecondaryIntervalSound ;
	
	[Space(10)]
		
		[Tooltip("How many Animations does the Animations Object have for the Secondary Fire? A weapon can have from 1 to 4 attack animations. By default, the first animation file must be named Fire_Secondary1, and each next: Fire_Secondary2, Fire_Secondary3 and Fire_Secondary4. 0 means there are no animations.")]
	 public int  MaxSecondaryAttackAnimations ;
	
	[Space(10)]
		
		[Tooltip("An attack can emit a continous sound (like a flamethrower), while the Attack Input is being pressed. If you wish to use this sound, put it's clip here. [IMPORTANT!] Using Continous sound clip with Fire Sound (below) might cause someunforseen consequences. It's not adviced to use both of these options at one. [IMPORTANT 2!] This option will not work with Semi firing mode (so only automatic weapons can use it).")]
	 public AudioClip SecondaryConstantSound ;
	
	[Tooltip("How many sound clips does the Secondary Fire have?")]
	 public int  MaxSecondaryFireSounds ;
	 public AudioClip SecondaryFireSound1 ;
	 public AudioClip SecondaryFireSound2 ;
	 public  AudioClip SecondaryFireSound3 ;
	 public AudioClip SecondaryFireSound4 ;
	 public AudioClip SecondaryFireSound5 ;
	
	[Tooltip("How many sound clips does the Secondary Fire Enviroment Hit have to randomize from? It's used by Raycast and Damage Area attacks - this sound is played, when they hit anything else than a Target (f.e. a wall).")]
	 public  int  MaxSecondaryEnviromentHitSounds ;
	 public AudioClip SecondaryEnviromentHitSound1 ;
	 public AudioClip SecondaryEnviromentHitSound2 ;
	 public AudioClip SecondaryEnviromentHitSound3 ;
	 public AudioClip SecondaryEnviromentHitSound4 ;
	 public AudioClip SecondaryEnviromentHitSound5 ;
	
	[Tooltip("How many sound clips does the Secondary Fire Target Hit have to randomize from? It's used by Raycast and Damage Area attacks - this sound is played, when they hit a Target")]
	 public  int  MaxSecondaryTargetHitSounds ;
	 public AudioClip SecondaryTargetHitSound1 ;
	 public AudioClip SecondaryTargetHitSound2 ;
	 public AudioClip SecondaryTargetHitSound3 ;
	 public AudioClip SecondaryTargetHitSound4 ;
	 public AudioClip SecondaryTargetHitSound5 ;
	
	[Space (10)]
		
		[Tooltip("Where will the muzzle flash (fire from barrel, smoke, magical sparks, etc.) appear, if the weapon has it? Create an empty GameObject, make it a child of this weapon's Prefab and place it around your weapon in the desired position. If weapon doesn't have any muzzle flash (like an axe or a silenced pistol), leave this field empty")]
	 public Transform  SecondaryFlashSourcePoint ; 
	
	[Tooltip("The muzzle flash to show after fire (a GameObject or Prefab) if weapon has it. If weapon doesn't have any muzzle flash (like an axe or a silenced pistol), leave this field empty")]
	 public GameObject  SecondaryMuzzleFlash ;
	
	[Space (10)]
		
		[Tooltip("Does the Secondary fire eject any GameObjects upon firing? (like empty cases and shells)")]
	 public bool SecondaryEjectsShells ;
	
	[Tooltip("From where will they be spawned? Create an empty GameObject, make it a child of this weapon's Prefab and place it around your weapon in the desired position. The shells are spawned according to z (blue) axis.")]
	 public  Transform SecondaryEjectedShellsSourcePoint ;
	
	[Tooltip("Prefab used as spawned shells.")]
	 public GameObject  SecondaryEjectedShells ;
	
	[Tooltip("Should the shells be propelled from start? If not, leave at zero. If they should, attach a rigidbody to the shell's GameObject!")]
	 public float SecondaryShellEjectionForce ;	
	
	
	[Space (20)]
		
		[Tooltip("Does this weapon fire Raycasts (a.k.a. Hitscan weapon - instant hiting, like ordinary firearms)?")]
	 public bool SecondaryFiresRaycast ;
	
	[Tooltip("What Layers should this weapon hit and which should be ignored (shot through) in Raycast (hitscan) Attacks? [HINT:]  Make sure it doesn't hit the Layer in which the Weapon model is (so you won't hit your weapon right after firing) and the Layer in which the Player is (so you won't hit yourself)!")]
	 public  LayerMask SecondaryRayTargetLayers  ;
	
	[Tooltip("What's the Tag of the Objects which are considered the Targets (which use Blood and Target Hit Sound, described in later variables)? [HINT:] For Example: Enemy or Weapon_Target.")]
	 public string SecondaryTargetTag  = "Enemy";
	
	[Tooltip("If it fires Raycasts, from where will they come from? Create an empty GameObject, make it a child of your Weapon Prefab and place it in a desired place around your weapon to specify this point - the attack will go along the Blue (z) axis.")]
	 public Transform  SecondaryRayBarrel ;
	
	[Tooltip("The GameObject or Prefab which appears, when Raycast hits an Object marked with Target Tag. (for example a blood splatter, robot parts flying, a burst of alien goo, etc.)")]
	 public GameObject  SecondaryBloodSplat ;
	
	[Tooltip("The GameObject or Prefab which appears, when Raycast hits something not signed with a weapon target Tag - for example a wall. (Bullet decals, sparks, splinters, etc)")]
	 public GameObject  SecondarySparks ;
	
	[Tooltip("If it fires Raycast, how much Damage will it deal? [ADVANCED:] Damage is sent to ApplyDamage()  void in the BM Health System script, as a Vector2(SecondaryRayDamage,SecondaryDamageType) called DamagePack.")]
	 public float SecondaryRayDamage ;
	
	[Tooltip("[OPTIONAL] What type of damage will it deal? Damage type is specified with a number and it is used for Resistances against various Damage Types in the BM Health System script. Changing this variable is not mandatory.")]
	 public int  SecondaryDamageType ;
	
	[Tooltip("If it fires Raycast, what's the maximum range of the weapon? If it doesn't matter, leave a high number, like 99999. Must be greater than zero!")]
	 public float SecondaryRayRange  = 100000;
	
	[Tooltip("Do shots apply pushing or pulling force on Rigidbodies?")]
	 public bool SecondaryRayAppliesForce ;
	
	[Tooltip("If it applies force, how strong will it be?")]
	 public float SecondaryRayForce ;
	
	[Space (5)]
		
		[Tooltip("Does this weapon create a sound in hit point, when Enviroment was hit with a raycast? (Like a sound of a ricocheting bullet)")]
	 public bool SecondaryRayHasEnviromentHitSounds ;
	
	[Tooltip("Does this weapon create a sound in hit point, when Target was hit with a raycast? (Like a sound of a robot parts being hit by a bullet or a blood splatter sound)")]	
	 public bool SecondaryRayHasTargetHitSounds ;
	
	
	[Space (20)]
		[Tooltip("Does this weapon fire a projectile or a damage area? (a fireball, a crossbow bolt, or a sword slash damage area, etc.)")]
	 public  bool SecondaryFiresProjectile ;
	
	[Tooltip("If it fires Projectiles, from where will they come from? Create an empty GameObject, make it a child of your Weapon Prefab and place it in a desired place around your weapon to specify this point - the Projectile will spawn along the Blue (z) axis. [HINT:] For normal Projectiles, set it around the barrel of your weapon. For Melee Damage Areas set it in the middle of the space where Damage Area will have it's center (like around the center of a sword's blade in the highest point of it's attack animation).")]
	 public  Transform SecondaryProjectileBarrel ;
	
	[Tooltip("Should a fired Projectile be free, or should it stay relative to the Projectile Barrel position? [HINT:]  A ranged weapon, like a Crossbow or a Rocket Launcher should have this checked False, because their projectiles move. A Sword Slash Damage Area or a Chainsaw Damage Area will have this checked True, so they stay in front of the weapon.")] 
	 public  bool StaticSecondaryProjectile  = false;
	
	[Tooltip("So... what's the Object or a Prefab used as a Projectile? [HINT:]  An object with BM Projectile System or BM Damage Area is advised, though not obligatory.")] 
	 public GameObject  SecondaryProjectile ;
	
	[Space (20)]
		
		[Tooltip("Does this weapon's Secondary Fire create Damage Area upon firing? (Action RPG melee weapons, explosions, AoE spells etc.)")]
	 public bool SecondaryFiresDamageArea ;
	[Tooltip("From where will the Attack be cast? Create an empty GameObject, make it a child of this weapon and place it in desired place to specify this point. All attacks will come from this point and all Angle calculations will be done using it's Z axis (blue) as front. If this field is left empty, Attack Origin will be this object's center.")]
	 public Transform  SecondaryDAAttackOrigin ;
	[Tooltip("What Layers of Objects should be affected by this Damage Area's damage and/or push? (like Targets, clutter, explosive barrels, destructible enviroment, wooden crates etc.)")]
	 public  LayerMask SecondaryDADamageLayers  ;
	[Tooltip("What is the range of this Damage Area?")]
	 public float SecondaryDARange ;
	[Tooltip("What is the Angle, in which Targets will be hit? [INFO:] The Angle is calculated from the Attack Origin center, through it's Z axis (blue, as front), to the Target's center. It's highly adviced to add some margin of error!")]
	 public float SecondaryDAAngle ;
	[Tooltip("What is the Damage of this Damage Area? [ADVANCED:] Damage is sent to ApplyDamage()  void in the BM Health System script, as a Vector2(SecondaryRayDamage,SecondaryDamageType) called DamagePack.")]
	 public float SecondaryDADamage ;
	[Tooltip("[OPTIONAL] What type of damage will it deal? Damage type is specified with a number and it is used for Resistances against various Damage Types in the BM Health System script. Changing this variable is not mandatory.")]
	 public  float SecondaryDADamageType ;
	[Tooltip("Should this Damage Area push or pull objects on Target Layers? The force is applied from the Attack Origin position.")] 
	 public bool SecondaryDAAppliesForce ;
	[Tooltip("If the above is True, how hard it does it push?")] 
	 public float  SecondaryDAForce ;
	[Tooltip("[BETA!] Should this Damage Area take obstacles into account? Targets obstructed by obstacles will not be hit.")]
	 public bool SecondaryPerformDAObstacleCheck ;
	[Tooltip("What Layers should be taken into consideration while Obstacle Checking? (That is: Targets and obstacles. For example the Enemies, enviroment, force fields, etc.)." )]
	 public  LayerMask SecondaryDAObstacleCheckLayers  ;
	[Tooltip("What object should be spawned upon hitting a Target? If none, leave empty.")]
	 public GameObject  SecondaryDABlood ;
	[Tooltip("What object should be spawned upon hitting an object which is not a Target (like a wall)? If none, leave empty.")]
	 public GameObject  SecondaryDAMissSparks ; 
	[Tooltip("Should this Damage Area simulate hitting enviroment? It's an additional Raycast (hitscan), going out of the front of the Attack Origin, spawning sparks on hit.")]
	 public bool SecondaryDASimulateEnviromentHit ;
	[Tooltip("What Layers can be hit by the above simuation?")]
	 public LayerMask  SecondaryDASimEnviroment  ;
	[Tooltip("Should this Secondary DA use Secondary Enviroment Hit Sounds?")]
	 public bool  SecondaryDAHasEviromentHitSounds ;
	[Tooltip("Should this Secondary DA use Secondary Target Hit Sounds?")]
	 public bool  SecondaryDAHasTargetHitSounds ;
	
	private List<GameObject> SecondaryAllTargets = new List<GameObject>() ;
	private List<GameObject> SecondaryTargets = new List<GameObject>() ;
	
	
	
	[Space(30)]
		[Header("[BETA] Projected Crosshair Options")]
			
			[Tooltip("Does this weapon have it's crosshair projected in 3D space, like a lasersight?")]
	 public bool  UseProjectedCrosshair ;
	
	[Tooltip("To what surfaces the Crosshair will stick to, and which it will shoot through? [HINT:]  Make sure it won't hit your Weapon and Projectiles layer.")]
	 public LayerMask  CrossHitAgainst  ;
	
	[Tooltip("From where will the crosshair be projected? Create an empty GameObject, make it a child of your Weapon Prefab and place it in a desired place around your weapon to specify this point - crosshair will be projected along the Blue (z) axis.")]
	 public Transform  CrosshairOrigin ;
	
	[Tooltip("[ADVANCED:] How far should the Raycast, checking for obstacles be? Changing ths is not mandatory and in most cases should be left with a high number.")]
	 public float ProjectionPrimaryRayRange  = 99999;
	
	
	[Tooltip("The object of this weapon's projected crosshair. Make sure it's a child of the Weapon Prefab and that is has no colliders attached!")]
	 public Transform  ProjectedCrosshair ;
	
	[Tooltip("How far should the crosshair be projected?")]
	 public  float ProjectedCrosshairRange ;
	
	[Tooltip("How far from the surface will the crosshair be projected? [HINT:]  if your Crosshair Object clips through slopes and irregular surfaces, set this variable higher")]
	 public float CrosshairPull  = 0.1f;
	
	
	[HideInInspector]
	 public int  ReadyToFire  = 0;
	
	 private  bool  BlockIdleAnimation ;

	 private  bool InitiatePrimaryCooldown ; 
	 private  bool HoldingFireP ;
	 private bool  HoldingFireS ;
	 private RaycastHit hit ;
	AudioSource _tempSound;
	
	
	[Space(10)]
		
		[Tooltip("Checking this option True and going into Play Mode will show a Debug of what has been hit with Raycast Attacks, for future testing.")]
	 public bool DebugHit ;
	
	
	
	 void Start()   //Let's start, shall we? In this  void we will set all the automatically assigned variables.
	{
		if (GetComponent<AudioSource >() != null) 
		{
			_tempSound = GetComponent<AudioSource > ();
		}
		
		if( PrimaryFiresDamageArea && PrimaryDAAttackOrigin == null ) //If primary fire attacks with a Damage Area and it's origin ("barrel")] is not specified, then make this Transform it's origin. 
		{
			PrimaryDAAttackOrigin = transform;
		}
		
		if(SecondaryFiresDamageArea && SecondaryDAAttackOrigin == null ) //The same as above, for the secondary fire.
		{
			SecondaryDAAttackOrigin = transform;
		}
		
		
		if(PrimaryFiresRaycast && PrimaryRayBarrel == null)  //if we're supposed to shoot Raycasts, but the barrel is not specified, poke the User to specify it.
		{
			Debug.LogError("Excuse me, but it seems that your weapon is set to fire Raycasts on Primary Fire while it doesn't have a Primary Ray Barrel. Please specify the Transform of the PrimaryRayBarrel variable in the Weapon System of "+transform.name+" to resume mayhem.");
		}	
		
		if(PrimaryFiresProjectile && PrimaryProjectileBarrel == null) //if we're supposed to shoot Projectiles, but the barrel is not specified, poke the User to specify it.
		{
			Debug.LogError("Well, I'm affraid that a Primary Projectile Barrel is still required to fire Primary Projectiles. Please specify the Transform for the PrimaryProjectileBarrel in the Weapon System of "+transform.name+", so the weapon knows from where to fire projectiles.");
		}	
		
		if(SecondaryFiresRaycast && SecondaryRayBarrel == null) //Same as just above, but for the secondary fire.
		{
			Debug.LogError("It looks like your weapon is trying to fire Raycasts as Secondary Fire mode, without a Secondary Raycast Barrel, which can be quite problematic. Please make sure that the SecondaryRayBarrel of the Weapon System on "+transform.name+" has a defined Transform, so the weapon knows from where to fire the Raycasts.");
		}	
		
		if(SecondaryFiresProjectile && SecondaryProjectileBarrel == null) //Again, secondary fire projectiles do not have a barrel. Let's tell the User to set it.
		{
			Debug.LogError("Nope, a Secondary Projectile Barrel is still required to fire Secondary Projectiles, nothing changed since the last version in this matter. Please make sure that the Secondary Projectile Barrel in the Weapon System of "+transform.name+" is set with an appropriate Transform, so the weapon knows from where to fire Secondary Projectiles.");
		}	
		
		
		if(SecondaryUsesPrimaryAmmo && UsesSecondaryAmmo) //One weapon can use just one type of ammo. If the user has set his Secondary Fire ammunition to use both Secondary and Primary supply, let's remind him to use just one of them.
		{
			Debug.LogError("Excuse me, but one of your Weapon Systems' Secondary Fire (on object named "+transform.name+", to be precise) is trying to use both UseSecondaryAmmo and SecondaryUsesPrimaryAmmo variables at once. That's highly unadviced, as it breaks the ammo checking calculations and may cause random glitches. Please, make sure that only one of these two options is set True.");  
		}
		
		//////////////
		//This here part makes sure, that the weapon uses just one firing mode for Primary and Secondary attack modes. If it uses more than one, Animation and ammo count glitches may occur.
		//The premise is simple - each Fire mode adds 1 to the FireModes variable. If it's more than 1, it means the user is trying to fire more than one mode at a time.

		 int  PFiremodes = 0;
		if(PrimaryFiresRaycast)
		{
			PFiremodes +=1;
		}
		
		if(PrimaryFiresProjectile)
		{
			PFiremodes +=1;
		}
		
		if(PrimaryFiresDamageArea)
		{
			PFiremodes +=1;
		}
		
		if(PFiremodes > 1)
		{
			Debug.LogWarning("I apologize, but "+transform.name+"'s Primary Fire Features are set to fire more than one fire mode at the same time (like Raycast + Projectile or Projectile + Damage Area, etc.). This may cause unexpected behaviour (like Animation glitches) and is overall not adviced. If you're truely sure to use both of them in one instance of the script, please, ignore this warning.", transform) ;
		}
		
		
		int SFiremodes = 0;
		if(SecondaryFiresRaycast)
		{
			SFiremodes +=1;
		}
		
		if(SecondaryFiresProjectile)
		{
			SFiremodes +=1;
		}
		
		if(SecondaryFiresDamageArea)
		{
			SFiremodes +=1;
		}
		
		if(SFiremodes > 1)
		{
			Debug.LogWarning("I apologize, but "+transform.name+"'s Secondary Fire Features are set to fire more than one fire mode at the same time (like Raycast + Projectile or Projectile + Damage Area, etc.). This may cause unexpected behaviour (like Animation glitches) and is overall not adviced. If you're truely sure to use both of them in one instance of the script, please, ignore this warning.", transform) ;
		}
		///////////
		
		
		
		//Initial Reload takes care of playing the spawning sound of the weapon, as well as the manual initial reload time - the time which has to pass after the weapon is spawned, before it can be fired.
		StartCoroutine ("InitialReload");
		
	}
	
	
	 void FixedUpdate()
	{
		//the Beta Projected Crosshair. It fires a raycast to check the distance and places the crosshair in the hit point accordingly.
		if(UseProjectedCrosshair)		
		{
			RaycastHit CrossHit ;
			if(Physics.Raycast(CrosshairOrigin.position, CrosshairOrigin.forward, out CrossHit, ProjectionPrimaryRayRange, CrossHitAgainst))
			{
				if(CrossHit.distance < ProjectedCrosshairRange) //if the crosshair's raycast hits something...
				{
					ProjectedCrosshair.transform.position = CrossHit.point - (CrosshairOrigin.forward * CrosshairPull);
					ProjectedCrosshair.transform.LookAt(CrosshairOrigin.transform);
				}
				else //if the above raycast hits but too far away...
				{
					ProjectedCrosshair.transform.position = CrosshairOrigin.position + (CrosshairOrigin.forward * (ProjectedCrosshairRange - CrosshairPull));
					ProjectedCrosshair.transform.LookAt(CrosshairOrigin.transform);
				}
			}
			else //and if it doesnt hit anything...
			{
				ProjectedCrosshair.transform.position = CrosshairOrigin.position + (CrosshairOrigin.forward * (ProjectedCrosshairRange - CrosshairPull));
				ProjectedCrosshair.transform.LookAt(CrosshairOrigin.transform);
			}
			
		}
	}
	
	 void Update () {
		
		
		if(animator != null)  //If we are using the Idle animations for this weapon, this part makes sure that the Idle clip is played when nothing is happening.
		{
			if (animator.GetComponent<Animation>().isPlaying == false && HasIdleAnimations == true && BlockIdleAnimation == false)
			{
				animator.GetComponent<Animation>().Play("Idle");
			}
		}
		
		//Inspect Animation Input
		
		if(HasInspectAnimation)
		{
			if(Input.GetButtonDown("Inspect") && (ReadyToFire == 1 || (PrimaryAmmo == 0 && SecondaryAmmo ==0)) && Reloading == false && inspecting == false)
			{
				StartCoroutine("Inspect");
			}
		}
		
		
		
		
		
		// Here the fire  voids begin.
		
		
		//Primary Fire
		///////////////
		
		if(PrimaryAmmo < 0)  //Let's make sure the Ammo doesnt go below zero...
		{
			PrimaryAmmo = 0;
		}
		
		if(PrimaryReloads == true && SparePrimaryAmmo > MaxPrimaryAmmo) //if we're using ammo clips, make sure that the Spare ammo doesn't exceed the Max Ammo.
		{
			SparePrimaryAmmo = MaxPrimaryAmmo;
		}
		
		if(PrimaryReloads == false && PrimaryAmmo > MaxPrimaryAmmo) //And if we're not using clips, make sure that the ammo clip doesnt exceed the Max Ammo.
		{
			PrimaryAmmo = MaxPrimaryAmmo;
		}
		
		
		//Primary Fire input.
		if(CanControl == true && HasPrimaryFire == true)
		{

			if (PrimarySemi == false )  //This part takes care of the Player Input on automatic fire mode.
			{ 
				if (Input.GetButton(PrimaryFireInput) && ReadyToFire == 1 && Time.timeScale != 0 && Reloading == false)
				{ 
					StartCoroutine("Fire_Primary"); //Ok, all seems to be in order, so let's fire our primary shot!	
					HoldingFireP = true; //is the player holding the button?		
					
					if(_tempSound != null && PrimaryConstantSound !=null && PrimarySemi == false && _tempSound.isPlaying == false) //play the constant fire sound, if it exists.
					{
						_tempSound.clip = PrimaryConstantSound;
						_tempSound.Play();
					}
				}
				
			}
			
			if (PrimarySemi == true) //That's a semi-automatic fire mode input check. 
			{
				if (Input.GetButtonDown(PrimaryFireInput) && ReadyToFire == 1 && Time.timeScale != 0 && Reloading == false)
				{ 
					StartCoroutine("Fire_Primary"); //Ok, all seems to be in order, so let's fire our primary shot!	
					HoldingFireP = true;
				}
			}
			
			
			//If there's no ammo, then play the dry fire sound, if it exists.
			if(Input.GetButtonDown(PrimaryFireInput) && PrimaryAmmo <= 0 && UsesPrimaryAmmo && PrimaryDryFireSound != null && DryFireReady == true && Reloading == false && inspecting == false)
			{
				StartCoroutine("DryPrimaryFire");
			}
			
		}
		
		//Primary Reload input check.
		if(PrimaryReloads && Input.GetButtonDown("Reload Primary") && SparePrimaryAmmo != 0 && PrimaryAmmo != PrimaryAmmoClipSize && Reloading == false)
		{
			StartCoroutine("ReloadPrimary");
		}
		
		/////////////
		
		
		
		
		
		
		
		//Secondary Fire
		///////////////
		if(SecondaryAmmo < 0)//Do not let the ammo drop below 0.
		{
			SecondaryAmmo = 0;
		}
		
		if(SecondaryReloads == true && SpareSecondaryAmmo > MaxSecondaryAmmo)	//if it uses ammo clips, do not let the spare ammo to exceed Max Ammo.
		{
			SpareSecondaryAmmo = MaxSecondaryAmmo;
		}
		
		if(SecondaryReloads == false && SecondaryAmmo > MaxSecondaryAmmo)	//if we're not using clips, do not let the ammo in the clip to exceeed the Max Ammo.
		{
			SecondaryAmmo = MaxSecondaryAmmo;
		}
		
		
		
		
		//Secondary Fire Input
		if(CanControl == true && HasSecondaryFire == true)
		{
			
			if (SecondarySemi == false) //Input for automatic fire
			{	 		
				if (Input.GetButton(SecondaryFireInput) && ReadyToFire == 1 && Time.timeScale != 0 && Reloading == false)
				{ 
					StartCoroutine("Fire_Secondary");//Ok, all seems to be in order, so let's fire our Secondary shot!			
					HoldingFireS = true; //is the player holding the button?
					
					if(_tempSound != null && SecondaryConstantSound != null && SecondarySemi == false && _tempSound.isPlaying == false) //play the constant sound when the user is attacking.
					{	
						_tempSound.clip = SecondaryConstantSound;
						_tempSound.Play();
					}
				}
			}
			
			
			if (SecondarySemi == true) //Input for semi-automatic fire.
			{
				if (Input.GetButtonDown(SecondaryFireInput) && ReadyToFire == 1 && Time.timeScale != 0 && Reloading == false)
				{ 
					StartCoroutine("Fire_Secondary"); //Ok, all seems to be in order, so let's fire our Secondary shot!	
					HoldingFireS = true; 
				}
			}
			

			if(SecondaryUsesPrimaryAmmo == false)
			{
			//If there's no ammo, play the dry fire sound.
			if(Input.GetButtonDown(SecondaryFireInput) && SecondaryAmmo <= 0 && UsesSecondaryAmmo && SecondaryDryFireSound != null && DryFireReady == true && Reloading == false && inspecting == false)
			{
				StartCoroutine("DrySecondaryFire");
			}
			}

			if(SecondaryUsesPrimaryAmmo)
			{
				if(Input.GetButtonDown(SecondaryFireInput) && PrimaryAmmo <= 0 && SecondaryDryFireSound != null && DryFireReady == true && Reloading == false && inspecting == false)
				{
					StartCoroutine("DrySecondaryFire");
				}
			}

		}
		
		//Secondary reload input
		if(SecondaryReloads && Input.GetButtonDown("Reload Secondary") && SpareSecondaryAmmo != 0 && SecondaryAmmo != SecondaryAmmoClipSize && Reloading == false)
		{	
			StartCoroutine("ReloadSecondary");
		}
		
		
		
		
		//If there are any constant attack sounds, stop them when the player releases the trigger.
		if(Input.GetButtonUp(PrimaryFireInput) || (UsesPrimaryAmmo && PrimaryAmmo <= 0))
		{
			HoldingFireP = false;
			if(PrimaryConstantSound != null)
			{
				_tempSound.Stop();
			}
		}
		
		if(Input.GetButtonUp(SecondaryFireInput) || (UsesSecondaryAmmo && SecondaryAmmo <= 0))
		{
			HoldingFireS = false;
			if(SecondaryConstantSound != null)
			{
				_tempSound.Stop();
			}
		}
		
	}
	
	
	
	 IEnumerator Inspect()  //This  void takes care of the Inspect animation and sound.
	{
		StopCoroutine("PrimaryCooldown");		//"feel free to interrupt the attack cooldown animations."
		StopCoroutine("SecondaryCooldown");
		
		inspecting = true;
		
		if(_tempSound != null && InspectSound != null) //if there's any Inspect sounds, play them.
		{
			_tempSound.clip = InspectSound;
			_tempSound.Play();
		}

		animator.GetComponent<Animation>().Play("Inspect");
		while ( animator.GetComponent<Animation>().IsPlaying("Inspect") ) yield return null;
		yield return new WaitForSeconds(0.5f);
		inspecting = false;
	}
	
	
	
	
	
	 public IEnumerator Fire_Primary() //That is the Primary Fire  void. It's nearly the same as Secondary Fire, so for the code comments reference the Primary Fire  voids.
	{

		if(ReadyToFire == 1)
		{
			bool NotFiring  = true;	//Ok, the shot is being fired, so let's hold the control over the trigger until another shot has permition to be fired.
			ReadyToFire = 0;
			
			

			
			//Let's stop all the other coroutines until the shot is done.
			/////////////
			inspecting = false;
			Reloading = false;
			
			StopCoroutine("Inspect");
			StopCoroutine("ReloadPrimary");
			StopCoroutine("ReloadSecondary");
			StopCoroutine("PrimaryCooldown");
			StopCoroutine("SecondaryCooldown");
			/////////
			if(PrimaryReloads && AutoReloadPrimary == true && PrimaryAmmo != PrimaryAmmoClipSize &&  SparePrimaryAmmo != 0 && PrimaryAmmo == 0 && Reloading == false)//So you fired, but your clip was empty? If Autoreload is on, then I'll reload it for you.
			{
				HoldingFireP = false;
				StartCoroutine("ReloadPrimary"); 
				//yield return false;
			}
			
			
			//Let's get to the ammo chceck.
			////////
			
			if(!UsesPrimaryAmmo) //if we're not using ammo, then let's proceed.
			{
				if(PrimaryHasWarmUpAnimation && HoldingFireP == false) //if there is a WarmUp animation (like minigun spin or spell incantation), let's play it.
				{
					animator.GetComponent<Animation>().Play("WarmUp_Primary");
					
					if(PrimaryWarmUpSound != null)
					{
						_tempSound.clip = PrimaryWarmUpSound;
						_tempSound.Play();
					}
					while ( animator.GetComponent<Animation>().IsPlaying("WarmUp_Primary") ) yield return null; //let's hold the rest of this  void until the WarmUp is done.
					animator.GetComponent<Animation>().Stop();
				}
				StartCoroutine("Fire_Primary_After_Ammo_Check"); //All is fine, let's fire a shot after the ammo check!
				NotFiring = false;
			}
			
			
			if(UsesPrimaryAmmo) //It's the same as above, just for firing with ammo. 
			{
				if(PrimaryAllowIncompleteLastShot == true && PrimaryAmmo > 0)  //Warmup Animation.
				{
					
					if(PrimaryHasWarmUpAnimation && HoldingFireP == false)
					{	
						animator.GetComponent<Animation>().Play("WarmUp_Primary");
						
						if(PrimaryWarmUpSound != null)
						{
							_tempSound.clip = PrimaryWarmUpSound;
							_tempSound.Play();
						}
						
						while ( animator.GetComponent<Animation>().IsPlaying("WarmUp_Primary") ) yield return null;
						
					}
					
					
					PrimaryAmmo -= PrimaryAmmoPerShot;  //Let's take the ammo from the Ammo clip.
					
					StartCoroutine("Fire_Primary_After_Ammo_Check"); //OK, let's fire after ammo check!
					NotFiring = false;
				}
				
				
				
				if(PrimaryAllowIncompleteLastShot == false && PrimaryAmmo >= PrimaryAmmoPerShot)
				{
					if(PrimaryHasWarmUpAnimation && HoldingFireP == false) //WarmUp animation and sound.
					{
						animator.GetComponent<Animation>().Play("WarmUp_Primary");
						
						if(PrimaryWarmUpSound != null)
						{
							_tempSound.clip = PrimaryWarmUpSound;
							_tempSound.Play();
						}
						while ( animator.GetComponent<Animation>().IsPlaying("WarmUp_Primary") ) 
						{
							yield return new WaitForEndOfFrame();
						}
						animator.GetComponent<Animation>().Stop();
						
					}
					
					PrimaryAmmo -= PrimaryAmmoPerShot; //aaaand let's take some ammo from the ammo clip.
					StartCoroutine("Fire_Primary_After_Ammo_Check");
					
					NotFiring = false;
				}
				
			}
			///////////	
			
			
			
			if(NotFiring == true) //If nothing happened, then we can return to the original state.
			{
				ReadyToFire = 1;
			}
		}
	}
	
	
	
	
	 IEnumerator Fire_Primary_After_Ammo_Check() //So we checked for ammo, so let's create the attack, shall we?
	{
		
		yield return new  WaitForSeconds(PrimaryManualFireDelay);
		
		if(animator != null)   //If there's anything playing on Animations Object, let's stop it to make space for other animations.
		{
			animator.GetComponent<Animation>().Rewind();   
		}
		
		FireSound_Primary();  //Let's play a fire sound.
		Primary_Fire_Animation();// And an animation too!
		
		yield return new WaitForSeconds(PrimaryShotDelay);  //So we've got Animations and sound running, let's wait for permition to fire...
		
		
		
		
		if(PrimaryFiresProjectile)   //if it fires projectile...
		{
			for (int i = 0; i < PrimaryPelletsPerShot; i++)
			{
				GameObject PrimaryProjClone ;   //creating a projectile!
				PrimaryProjClone = Instantiate(PrimaryProjectile, PrimaryProjectileBarrel.position, PrimaryProjectileBarrel.rotation) as GameObject;
				
				if(StaticPrimaryProjectile)
				{
					PrimaryProjClone.transform.parent = PrimaryProjectileBarrel;   //if the projectile is static, let's pivot it to the parent.
				}
				
				PrimaryProjClone.transform.Rotate(Random.Range(-PrimaryVerticalBulletSpread,PrimaryVerticalBulletSpread), Random.Range(-PrimaryHorizontalBulletSpread,PrimaryHorizontalBulletSpread), 0);  //This here line adds the bullet spread.
			}
			
		}
		
		
		
		
		if(PrimaryFiresRaycast)  //if it fires raycast...
		{
			
			
			for (int i1 = 0; i1 < PrimaryPelletsPerShot; i1++)
			{ 
				Vector3 direction   = PrimaryRayBarrel.forward;   //these lines add the bullet spread.
				direction += Random.Range(-PrimaryHorizontalBulletSpread, PrimaryHorizontalBulletSpread) * PrimaryRayBarrel.right;
				direction += Random.Range(-PrimaryVerticalBulletSpread, PrimaryVerticalBulletSpread) * PrimaryRayBarrel.up;
				
				var ForceDir = direction.normalized;  
				GameObject  PrimaryRayCloneSparks ;
				
				if (Physics.Raycast (PrimaryRayBarrel.position, direction, out hit, PrimaryRayRange, PrimaryRayTargetLayers))  //The Raycast is being shot!
				{
					
					if(DebugHit == true)
					{
						Debug.Log("DEBUG: Primary Fire of "+transform.name+" has hit an object named "+ hit.transform.name+".");
					}
					
					
					if(hit.transform != null)   //so it hit something...
					{
						if(hit.transform.tag != PrimaryTargetTag)  //...and it's not a Target.
						{
							DamagePack = new Vector2(PrimaryRayDamage,PrimaryDamageType);
							hit.transform.SendMessage("ApplyDamage", DamagePack, SendMessageOptions.DontRequireReceiver);
							
							//Apply Force
							if(PrimaryRayAppliesForce == true && hit.rigidbody != null) //applying force!
							{
								hit.rigidbody.AddForce(ForceDir * PrimaryRayForce, ForceMode.Impulse);
							}
							
							
							if(PrimarySparks != null) //and letting off some sparks.
							{
								PrimaryRayCloneSparks = Instantiate(PrimarySparks, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
								PrimaryRayCloneSparks.transform.Translate(Vector3.forward * 0.1f);
								PrimaryRayCloneSparks.transform.LookAt(PrimaryRayBarrel);
							}
							
							if(PrimaryRayHasEnviromentHitSounds)  //and an Enviroment Hit sound.
							{
								EnvHitSound_Primary(hit);
							}
							
						}
						
						
						
						if(hit.transform.tag == PrimaryTargetTag) //...and we hit a Target! Woohoo!
						{
							DamagePack =new  Vector2(PrimaryRayDamage,PrimaryDamageType);
							hit.transform.SendMessage("ApplyDamage", DamagePack, SendMessageOptions.DontRequireReceiver);
							
							//Let's apply force!
							if(PrimaryRayAppliesForce == true && hit.rigidbody != null)
							{
								hit.rigidbody.AddForce(ForceDir * PrimaryRayForce, ForceMode.Impulse);
							}
							
							if(PrimaryBloodSplat != null)  //and spawn some blood!
							{
								GameObject  PrimaryProjCloneBlood ;
								PrimaryProjCloneBlood = Instantiate(PrimaryBloodSplat, hit.point, transform.rotation) as GameObject;
								PrimaryProjCloneBlood.transform.LookAt(transform);
							}
							if(PrimaryRayHasTargetHitSounds) //Lastly, let's play a Target hit sound.
							{
								TargetHitSound_Primary(hit);
							}
						}
					}
				}	
			}
		}
		
		
		if(PrimaryFiresDamageArea)  //if it fires off a damage area...
		{
			
			Collider[] _p = Physics.OverlapSphere(PrimaryDAAttackOrigin.position, PrimaryDARange, PrimaryDADamageLayers); //This is the main targeting code line. 
			for(int i = 0; i < _p.Length; i++)
			{
				PrimaryAllTargets.Add(_p[i].gameObject);
			}
	
			
			
			foreach(GameObject InitialT in PrimaryAllTargets) //looping through targets to do bad things to them one by one.
			{ 
				
				if(!PrimaryTargets.Contains(InitialT.gameObject) )
				{
					PrimaryTargets.Add(InitialT.gameObject);  //Add this particular Target to the array, to keep track of what we already hit...
					
					GameObject  Target  = InitialT.gameObject;		  
					Transform  TargetTransform  = Target.transform;
					Vector3 AttackDirection   = TargetTransform.position - PrimaryDAAttackOrigin.position;
					float CalculatedAngle  = PrimaryDAAngle /2;
					float TargetAngle  = Vector3.Angle(AttackDirection, PrimaryDAAttackOrigin.forward);
					RaycastHit hit ;
					Ray SRay ;
					
					if(PrimaryPerformDAObstacleCheck)  //Obstacle checking with a raycast.
					{
						RaycastHit OCHit ;
						if(Physics.Raycast(PrimaryDAAttackOrigin.position, AttackDirection, out OCHit, Mathf.Infinity, PrimaryDAObstacleCheckLayers))
						{
							if(OCHit.transform.gameObject != Target.gameObject)
							{
								Target = null;  //if there's an obstacle, clear this target off.
							}
						}
					}
					
					if(TargetAngle <= CalculatedAngle && Target != null && Target.tag == PrimaryTargetTag) //if we hit a valid, bleeding Target and the obstacle check was fine, let's at leat check the Angle.
					{
						DamagePack =new  Vector2(PrimaryDADamage,PrimaryDADamageType);
						Target.SendMessage("ApplyDamage", DamagePack, SendMessageOptions.DontRequireReceiver);
						
						if(PrimaryDAAppliesForce == true && Target.GetComponent<Rigidbody>() != null)  //and apply force to the target.
						{
							Target.GetComponent<Rigidbody>().AddForce(AttackDirection * PrimaryDAForce, ForceMode.Impulse);
						}
						
						if(PrimaryDABlood != null) //And spawn blood! How could someone forget about blood! 
						{
							GameObject  PrimaryProjCloneBlood1 ;
							Ray BloodRay  = new Ray(PrimaryDAAttackOrigin.position, AttackDirection);
							RaycastHit BloodHit ;
							Target.GetComponent<Collider>().Raycast(BloodRay, out BloodHit, Mathf.Infinity); 
							PrimaryProjCloneBlood1 = Instantiate(PrimaryDABlood, BloodHit.point, transform.rotation) as GameObject;
							PrimaryProjCloneBlood1.transform.LookAt(transform);
						}
						
						if(PrimaryDAHasTargetHitSounds) //and lastly, let's play a sound.
						{
							SRay = new Ray(PrimaryDAAttackOrigin.position, AttackDirection);
							Target.GetComponent<Collider>().Raycast(SRay, out hit, Mathf.Infinity); 
							TargetHitSound_Primary(hit);
						}
					}
					
					if(TargetAngle <= CalculatedAngle && Target != null && Target.tag != PrimaryTargetTag) //if we hit a non-Target object, let's check if the Angle is allright.
					{
						
						DamagePack =new  Vector2(PrimaryDADamage,PrimaryDADamageType);
						Target.SendMessage("ApplyDamage", DamagePack, SendMessageOptions.DontRequireReceiver);
						
						if(PrimaryDAAppliesForce == true && Target.GetComponent<Rigidbody>() != null) //and apply force.
						{
							Target.GetComponent<Rigidbody>().AddForce(AttackDirection * PrimaryDAForce, ForceMode.Impulse);
						}
						
						if(PrimaryDAMissSparks != null)  //and sprinkle some sparks...
						{
							GameObject  PrimaryProjCloneSparks ;
							Ray SparksRay  = new Ray(PrimaryDAAttackOrigin.position, AttackDirection);
							RaycastHit SparksHit ;
							Target.GetComponent<Collider>().Raycast(SparksRay, out SparksHit, Mathf.Infinity); 
							PrimaryProjCloneSparks = Instantiate(PrimaryDAMissSparks, SparksHit.point, transform.rotation) as GameObject;
							PrimaryProjCloneSparks.transform.LookAt(transform);
						}
						if(PrimaryDAHasEviromentHitSounds) //and play a sound.
						{
							SRay = new Ray(PrimaryDAAttackOrigin.position, AttackDirection);
							Target.GetComponent<Collider>().Raycast(SRay, out  hit, Mathf.Infinity); 
							EnvHitSound_Primary(hit);
						}	
					}
				}
			}
			
			
			PrimaryTargets.Clear();  //if everything is done, reset the damage area array of hit objects.
			
			PrimaryAllTargets.Clear(); //the second one too.
			
			
			if(PrimaryDASimulateEnviromentHit) //if we're simulating an enviroment hit with a raycast, it happens here.
			{
				GameObject  PrimaryProjCloneSparks1 ;
				RaycastHit PrimarySimSparkHit ;
				if(Physics.Raycast(PrimaryDAAttackOrigin.position, PrimaryDAAttackOrigin.forward,out  PrimarySimSparkHit, PrimaryDARange, PrimaryDASimEnviroment))
				{
					PrimaryProjCloneSparks1 = Instantiate(PrimaryDAMissSparks, PrimarySimSparkHit.point, transform.rotation) as GameObject;
					PrimaryProjCloneSparks1.transform.LookAt(transform);
				}
				hit = PrimarySimSparkHit;
				EnvHitSound_Primary(hit);
			}
			
			
		}
		
		
		
		
		if(PrimaryEjectsShells) //spawning and ejecting shells!
		{
			GameObject  PrimaryShell ;
			PrimaryShell = Instantiate(PrimaryEjectedShells, PrimaryEjectedShellsSourcePoint.position, PrimaryEjectedShellsSourcePoint.rotation) as GameObject;
			if(PrimaryShellEjectionForce != 0)
			{
				PrimaryShell.GetComponent<Rigidbody>().AddForce((PrimaryEjectedShellsSourcePoint.forward * PrimaryShellEjectionForce), ForceMode.Impulse);
			}
		}
		
		if (PrimaryMuzzleFlash != null) //Spawning a muzzle flash...
		{
			GameObject PrimaryMuzzleClone2 ;
			PrimaryMuzzleClone2 = Instantiate(PrimaryMuzzleFlash, transform.position, PrimaryFlashSourcePoint.transform.rotation) as GameObject;
			PrimaryMuzzleClone2.transform.position = PrimaryFlashSourcePoint.transform.position;
			PrimaryMuzzleClone2.transform.parent = PrimaryFlashSourcePoint.transform;
		}
		
		
		
		//Fire interval 
		yield return new WaitForSeconds(PrimaryFireInterval);
		
		
		
		
		if(PrimaryHasIntervalAnimation == true) //playing the interval animation.
		{
			animator.GetComponent<Animation>().Play("Interval_Primary"); 
			if(PrimaryIntervalSound != null)
			{
				_tempSound.clip =PrimaryIntervalSound;
				_tempSound.Play();
			}
			while (animator.GetComponent<Animation>().IsPlaying("Interval_Primary")) yield return new WaitForEndOfFrame();
		}
		
		if(PrimaryHasCooldownAnimation == true)
		{
			StartCoroutine("PrimaryCooldown");
		}
		
		ReadyToFire = 1; //Phew! That was a lot of calculations. If all is OK, let's give a green light for another shot.
		
		
	}
	
	
	

	
	
	
	
	
	
	
	public IEnumerator Fire_Secondary()  //it's exactely the same as Primary Fire, with just minor ammo tweaks. For reference, look for Primary Fire comments.
	{
		if(ReadyToFire == 1)
		{
			var NotFiring  = true;
			
			ReadyToFire = 0;
			

			

			
			
			
			inspecting = false;
			Reloading = false;
			StopCoroutine("Inspect");
			StopCoroutine("ReloadSecondary");
			StopCoroutine("ReloadPrimary");
			StopCoroutine("PrimaryCooldown");
			StopCoroutine("SecondaryCooldown");

			if(SecondaryUsesPrimaryAmmo && AutoReloadPrimary == true)   // The ammo tweaks are right here! ;)
			{
				if(PrimaryReloads  && SparePrimaryAmmo != 0 && PrimaryAmmo == 0 && Reloading == false)
				{
					HoldingFireS = false;
					StartCoroutine("ReloadPrimary");
					//yield return false;
				}
			}
			if(!SecondaryUsesPrimaryAmmo && AutoReloadSecondary == true)
			{
				if(SecondaryReloads  && SpareSecondaryAmmo != 0 && SecondaryAmmo != SecondaryAmmoClipSize && SecondaryAmmo == 0 && Reloading == false)
				{
					HoldingFireS = false;
					StartCoroutine("ReloadSecondary");
					//yield return false;
				}
			}
			
			if(!UsesSecondaryAmmo && !SecondaryUsesPrimaryAmmo)
			{ 
				if(SecondaryHasWarmUpAnimation && HoldingFireS == false)
				{
					animator.GetComponent<Animation>().Play("WarmUp_Secondary");
					
					if(SecondaryWarmUpSound != null)
					{
						_tempSound.clip = SecondaryWarmUpSound;
						_tempSound.Play();
					}
					while ( animator.GetComponent<Animation>().IsPlaying("WarmUp_Secondary") ) yield return new WaitForEndOfFrame();
					animator.GetComponent<Animation>().Stop();
					
				}
				
				StartCoroutine("Fire_Secondary_After_Ammo_Check");
				NotFiring = false;
				
			}
			
			
			if(UsesSecondaryAmmo)
			{
				if(SecondaryAllowIncompleteLastShot == true && SecondaryAmmo > 0)
				{
					
					if(SecondaryHasWarmUpAnimation && HoldingFireS == false)
					{	
						ReadyToFire = 0;
						animator.GetComponent<Animation>().Play("WarmUp_Secondary");
						
						if(SecondaryWarmUpSound != null)
						{
							_tempSound.clip = SecondaryWarmUpSound;
							_tempSound.Play();
						}
						
						while ( animator.GetComponent<Animation>().IsPlaying("WarmUp_Secondary") ) yield return new WaitForEndOfFrame();
						
					}
					SecondaryAmmo -= SecondaryAmmoPerShot;
					StartCoroutine("Fire_Secondary_After_Ammo_Check");
					NotFiring = false;
					
				}
				
				if(SecondaryAllowIncompleteLastShot == false && SecondaryAmmo >= SecondaryAmmoPerShot)
				{
					if(SecondaryHasWarmUpAnimation && HoldingFireS == false)
					{
						ReadyToFire = 0;
						animator.GetComponent<Animation>().Play("WarmUp_Secondary");
						
						if(SecondaryWarmUpSound != null)
						{
							_tempSound.clip =SecondaryWarmUpSound;
							_tempSound.Play();
						}
						while ( animator.GetComponent<Animation>().IsPlaying("WarmUp_Secondary") ) yield return new WaitForEndOfFrame();
						
					}
					
					SecondaryAmmo -= SecondaryAmmoPerShot; 
					StartCoroutine("Fire_Secondary_After_Ammo_Check");
					NotFiring = false;
					
				}
				
			}
			
			if(SecondaryUsesPrimaryAmmo)
			{
				if(SecondaryAllowIncompleteLastShot == true && PrimaryAmmo > 0)
				{
					
					if(SecondaryHasWarmUpAnimation && HoldingFireS == false)
					{	
						ReadyToFire = 0;
						animator.GetComponent<Animation>().Play("WarmUp_Secondary");
						
						if(SecondaryWarmUpSound != null)
						{
							_tempSound.clip = SecondaryWarmUpSound;
							_tempSound.Play();
						}
						
						while ( animator.GetComponent<Animation>().IsPlaying("WarmUp_Secondary") ) yield return new WaitForEndOfFrame();
						
					}
					PrimaryAmmo -= SecondaryAmmoPerShot;
					StartCoroutine("Fire_Secondary_After_Ammo_Check");
					NotFiring = false;
					
				}
				
				if(SecondaryAllowIncompleteLastShot == false && PrimaryAmmo >= SecondaryAmmoPerShot)
				{
					if(SecondaryHasWarmUpAnimation && HoldingFireS == false)
					{
						ReadyToFire = 0;
						animator.GetComponent<Animation>().Play("WarmUp_Secondary");
						
						if(SecondaryWarmUpSound != null)
						{
							_tempSound.clip = SecondaryWarmUpSound;
							_tempSound.Play();
						}
						while ( animator.GetComponent<Animation>().IsPlaying("WarmUp_Secondary") ) yield return new WaitForEndOfFrame();
						
					}
					
					PrimaryAmmo -= SecondaryAmmoPerShot; 
					StartCoroutine("Fire_Secondary_After_Ammo_Check");
					NotFiring = false;
				}
			}
			
			if(NotFiring == true)
			{
				ReadyToFire = 1;
			}
		}
	}
	
	 IEnumerator Fire_Secondary_After_Ammo_Check() //Same as above - it's pretty much the same as Primary Fire  voids.
	{
		yield return new WaitForSeconds(SecondaryManualFireDelay);
		if(animator != null)
		{
			animator.GetComponent<Animation>().Rewind();
		}
		FireSound_Secondary();
		Secondary_Fire_Animation();
		
		yield return new WaitForSeconds(SecondaryShotDelay);
		
		
		
		if(SecondaryFiresProjectile)
		{
			for (int i = 0; i < SecondaryPelletsPerShot; i++)
			{
				GameObject  SecondaryProjClone ; 
				SecondaryProjClone = Instantiate(SecondaryProjectile, SecondaryProjectileBarrel.position, SecondaryProjectileBarrel.rotation) as GameObject;
				
				if(StaticSecondaryProjectile)
				{
					SecondaryProjClone.transform.parent = SecondaryProjectileBarrel;
				}
				
				SecondaryProjClone.transform.Rotate(Random.Range(-SecondaryVerticalBulletSpread,SecondaryVerticalBulletSpread), Random.Range(-SecondaryHorizontalBulletSpread,SecondaryHorizontalBulletSpread), 0);
			}
			
		}
		
		
		
		
		if(SecondaryFiresRaycast)
		{
			
			
			for (int i1 = 0; i1 < SecondaryPelletsPerShot; i1++)
			{ 
				Vector3 direction   = SecondaryRayBarrel.forward;
				direction += Random.Range(-SecondaryHorizontalBulletSpread, SecondaryHorizontalBulletSpread) * SecondaryRayBarrel.right;
				direction += Random.Range(-SecondaryVerticalBulletSpread, SecondaryVerticalBulletSpread) * SecondaryRayBarrel.up;
				
				Vector3 ForceDir = direction.normalized;
				GameObject SecondaryRayCloneSparks ;
				
				if (Physics.Raycast (SecondaryRayBarrel.position, direction, out hit, SecondaryRayRange, SecondaryRayTargetLayers))  //The Raycast is being shot!
				{
					
					if(DebugHit == true)
					{
						Debug.Log("DEBUG: Secondary Fire of "+transform.name+" has hit an object named "+ hit.transform.name+".");
					}
					
					if(hit.transform != null)
					{
						
						//Wall hit
						if(hit.transform.tag != SecondaryTargetTag)
						{
							DamagePack =new  Vector2(SecondaryRayDamage,SecondaryDamageType);
							hit.transform.SendMessage("ApplyDamage", DamagePack, SendMessageOptions.DontRequireReceiver);
							
							//Apply Force
							if(SecondaryRayAppliesForce == true && hit.rigidbody != null)
							{
								hit.rigidbody.AddForce(ForceDir * SecondaryRayForce, ForceMode.Impulse);
							}
							
							
							if(SecondarySparks != null)
							{
								SecondaryRayCloneSparks = Instantiate(SecondarySparks, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
								SecondaryRayCloneSparks.transform.Translate(Vector3.forward * 0.1f);
								SecondaryRayCloneSparks.transform.LookAt(SecondaryRayBarrel);
								
							}
							
							if(SecondaryRayHasEnviromentHitSounds)
							{
								EnvHitSound_Secondary(hit);
							}
							
						}
						
						
						//Target hit
						if(hit.transform.tag == SecondaryTargetTag)
						{
							DamagePack =new  Vector2(SecondaryRayDamage,SecondaryDamageType);
							hit.transform.SendMessage("ApplyDamage", DamagePack, SendMessageOptions.DontRequireReceiver);
							
							//Apply Force	
							if(SecondaryRayAppliesForce == true && hit.rigidbody != null)
							{
								hit.rigidbody.AddForce(ForceDir * SecondaryRayForce, ForceMode.Impulse);
							}
							
							if(SecondaryBloodSplat != null)
							{
								GameObject SecondaryProjCloneBlood ;
								SecondaryProjCloneBlood = Instantiate(SecondaryBloodSplat, hit.point, transform.rotation) as GameObject;
								SecondaryProjCloneBlood.transform.LookAt(transform);
							}
							if(SecondaryRayHasTargetHitSounds)
							{
								TargetHitSound_Secondary(hit);
							}
						}		
					}
				}	
			}
		}
		
		if(SecondaryFiresDamageArea)
		{

			Collider[] _s = Physics.OverlapSphere(SecondaryDAAttackOrigin.position, SecondaryDARange, SecondaryDADamageLayers);
			for(int i = 0; i<_s.Length; i++)
			{	
				SecondaryAllTargets.Add(_s[i].gameObject);
			}


			
			if(DebugHit == true)
			{
				Debug.Log("Damage Area of "+transform.name +" has just hit "+SecondaryAllTargets.Count+" objects (on Damage Layers)...");
			}
			
			foreach(GameObject InitialT in SecondaryAllTargets)
			{ 	
				
				
				
				if(!SecondaryTargets.Contains(InitialT.gameObject) )
				{	
					SecondaryTargets.Add(InitialT.gameObject);
					
					GameObject Target  = InitialT.gameObject;		  
					Transform TargetTransform  = Target.transform;
					Vector3 AttackDirection   = TargetTransform.position - SecondaryDAAttackOrigin.position;
					float CalculatedAngle  = SecondaryDAAngle /2;
					float TargetAngle  = Vector3.Angle(AttackDirection, SecondaryDAAttackOrigin.forward);
					RaycastHit hit ;
					Ray SRay ;
					
					
					if(SecondaryPerformDAObstacleCheck)
					{	
						RaycastHit OCHit ;
						if(Physics.Raycast(SecondaryDAAttackOrigin.position, AttackDirection,out  OCHit, Mathf.Infinity, SecondaryDAObstacleCheckLayers))
						{
							if(OCHit.transform.gameObject != Target.gameObject)
							{
								Target = null;
							}
						}
					}
					if(TargetAngle <= CalculatedAngle && Target != null && Target.tag == SecondaryTargetTag)
					{	
						Vector2 DamagePack =new  Vector2(SecondaryDADamage,SecondaryDADamageType);
						Target.SendMessage("ApplyDamage", DamagePack, SendMessageOptions.DontRequireReceiver);
						
						if(DebugHit == true)
						{
							Debug.Log("Object named "+Target.name+" got hit by Damage Area of "+transform.name+" for "+DamagePack.x +" Damage points of type "+DamagePack.y+".");
						}
						
						
						if(SecondaryDAAppliesForce == true && Target.GetComponent<Rigidbody>() != null)
						{	
							Target.GetComponent<Rigidbody>().AddForce(AttackDirection * SecondaryDAForce, ForceMode.Impulse);
						}
						
						if(SecondaryDABlood != null)
						{
							GameObject SecondaryProjCloneBlood1 ;
							Ray BloodRay  = new Ray(SecondaryDAAttackOrigin.position, AttackDirection);
							RaycastHit BloodHit ;
							Target.GetComponent<Collider>().Raycast(BloodRay, out BloodHit, Mathf.Infinity); 
							SecondaryProjCloneBlood1 = Instantiate(SecondaryDABlood, BloodHit.point, transform.rotation) as GameObject;
							SecondaryProjCloneBlood1.transform.LookAt(transform);
						}
						if(SecondaryDAHasTargetHitSounds)
						{
							SRay = new Ray(SecondaryDAAttackOrigin.position, AttackDirection);
							Target.GetComponent<Collider>().Raycast(SRay,out  hit, Mathf.Infinity); 
							TargetHitSound_Secondary(hit);
						}
					}
					
					if(TargetAngle <= CalculatedAngle && Target != null && Target.tag != SecondaryTargetTag)
					{

						DamagePack =new  Vector2(SecondaryDADamage,SecondaryDADamageType);
						Target.SendMessage("ApplyDamage", DamagePack, SendMessageOptions.DontRequireReceiver);
						
						if(DebugHit == true)
						{
							Debug.Log("Object named "+Target.name+" got hit by Damage Area of "+transform.name+" for "+DamagePack.x +" Damage points of type "+DamagePack.y+".");
						}
						
						if(SecondaryDAAppliesForce == true && Target.GetComponent<Rigidbody>() != null)
						{
							Target.GetComponent<Rigidbody>().AddForce(AttackDirection * SecondaryDAForce, ForceMode.Impulse);
						}
						
						if(SecondaryDAMissSparks != null)
						{
							GameObject SecondaryProjCloneSparks ;
							Ray SparksRay  = new Ray(SecondaryDAAttackOrigin.position, AttackDirection);
							RaycastHit SparksHit ;
							Target.GetComponent<Collider>().Raycast(SparksRay,out  SparksHit, Mathf.Infinity); 
							SecondaryProjCloneSparks = Instantiate(SecondaryDAMissSparks, SparksHit.point, transform.rotation) as GameObject;
							SecondaryProjCloneSparks.transform.LookAt(transform);
						}
						if(SecondaryDAHasEviromentHitSounds)
						{
							SRay = new Ray(SecondaryDAAttackOrigin.position, AttackDirection);
							Target.GetComponent<Collider>().Raycast(SRay,out  hit, Mathf.Infinity); 
							EnvHitSound_Secondary(hit);
						}	
					}
				}
			}
			
			
			SecondaryTargets.Clear();  //delete this for Remote Damage Area
			SecondaryAllTargets.Clear ();
			
			if(SecondaryDASimulateEnviromentHit)
			{
				GameObject SecondaryProjCloneSparks1 ;
				RaycastHit SecondarySimSparkHit ;
				if(Physics.Raycast(SecondaryDAAttackOrigin.position, SecondaryDAAttackOrigin.forward, out SecondarySimSparkHit, SecondaryDARange, SecondaryDASimEnviroment))
				{
					SecondaryProjCloneSparks1 = Instantiate(SecondaryDAMissSparks, SecondarySimSparkHit.point, transform.rotation) as GameObject;
					SecondaryProjCloneSparks1.transform.LookAt(transform);
				}
				hit = SecondarySimSparkHit;
				EnvHitSound_Secondary(hit);
			}
			
			
		}
		
		if(SecondaryEjectsShells)
		{
			GameObject SecondaryShell ;
			SecondaryShell = Instantiate(SecondaryEjectedShells, SecondaryEjectedShellsSourcePoint.position, SecondaryEjectedShellsSourcePoint.rotation) as GameObject;
			if(SecondaryShellEjectionForce != 0)
			{
				SecondaryShell.GetComponent<Rigidbody>().AddForce((SecondaryEjectedShellsSourcePoint.forward * SecondaryShellEjectionForce), ForceMode.Impulse);
			}
		}
		
		if (SecondaryMuzzleFlash != null)
		{
			GameObject SecondaryMuzzleClone2 ;
			SecondaryMuzzleClone2 = Instantiate(SecondaryMuzzleFlash, transform.position, SecondaryFlashSourcePoint.transform.rotation) as GameObject;
			SecondaryMuzzleClone2.transform.position = SecondaryFlashSourcePoint.transform.position;
			SecondaryMuzzleClone2.transform.parent = SecondaryFlashSourcePoint.transform;
		}
		
		
		
		//Fire interval 
		yield return new WaitForSeconds(SecondaryFireInterval);
		
		
		if(SecondaryHasIntervalAnimation == true)
		{
			animator.GetComponent<Animation>().Play("Interval_Secondary"); 
			if(SecondaryIntervalSound != null)
			{
				_tempSound.clip = SecondaryIntervalSound;
				_tempSound.Play();
			}
			while (animator.GetComponent<Animation>().IsPlaying("Interval_Secondary")) yield return new WaitForEndOfFrame();
		}
		
		if(SecondaryHasCooldownAnimation == true)
		{
			if(!SecondaryHasPrimaryCooldown)
			{
				StartCoroutine("SecondaryCooldown");
			}
			
			if(SecondaryHasPrimaryCooldown)
			{
				StartCoroutine("PrimaryCooldown");
			}
		}
		
		ReadyToFire = 1;
		
		
	}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	 IEnumerator ReloadPrimary()  //I heard you want to reload your primary fire mode. Let's do it here...
	{
		Reloading = true;
		StopCoroutine("Inspect");
		inspecting = false;
		ReadyToFire = 0;
		StopCoroutine("Fire_Primary_After_Ammo_Check");     //Stopping all other  voids and coroutines.
		StopCoroutine("Fire_Secondary_After_Ammo_Check");
		StopCoroutine("PrimaryCooldown");
		StopCoroutine("SecondaryCooldown");
		Reloading = true;
		
		if(PrimaryHasReloadAnimation && animator != null) //playing animation...
		{
			animator.GetComponent<Animation>().Play("Reload_Primary");
		}
		if(PrimaryReloadSound != null)  //playing sound...
		{
			_tempSound.clip = PrimaryReloadSound;
			_tempSound.Play();
		}
		
		if(PrimaryHasReloadAnimation && animator != null)
		{
			float YieldTime  = animator.GetComponent<Animation>()["Reload_Primary"].length;   //wait for the Animations to end...
			yield return new WaitForSeconds (YieldTime);
		}
		
		yield return new WaitForSeconds (PrimaryManualReloadTime);
		
		
		int HowMuchWasInTheClip  = PrimaryAmmo;  //keeping track on the ammo count.
		
		if(SparePrimaryAmmo >= PrimaryAmmoClipSize)    //...and calculating it.
		{	
			PrimaryAmmo = PrimaryAmmoClipSize;
			SparePrimaryAmmo -= PrimaryAmmoClipSize;
			SparePrimaryAmmo += HowMuchWasInTheClip;
		}
		
		if(SparePrimaryAmmo < PrimaryAmmoClipSize)
		{
			PrimaryAmmo += SparePrimaryAmmo;
			
			if(PrimaryAmmo > PrimaryAmmoClipSize)
			{
				SparePrimaryAmmo = (PrimaryAmmo - PrimaryAmmoClipSize);
				PrimaryAmmo = PrimaryAmmoClipSize;
			}
			else
			{
				SparePrimaryAmmo = 0;	
			}
		}
		
		
		if(InfinitePrimaryMaxAmmo == true)  //if there's infinite ammo, just drop it.
		{	
			SparePrimaryAmmo = PrimaryAmmoClipSize;
		}
		
		Reloading = false;
		ReadyToFire = 1; //OK, green light for another shot!
	}
	
	
	
	
	 IEnumerator ReloadSecondary() //it's the same as Primary Reload, just right above. for comments, reference the above  void.
	{
		Reloading = true;
		StopCoroutine("Inspect");
		inspecting = false;
		ReadyToFire = 0;
		StopCoroutine("Fire_Primary_After_Ammo_Check");
		StopCoroutine("Fire_Secondary_After_Ammo_Check");
		StopCoroutine("PrimaryCooldown");
		StopCoroutine("SecondaryCooldown");

		Reloading = true;
		if(SecondaryHasReloadAnimation)
		{		

			animator.GetComponent<Animation>().Play("Reload_Secondary");
		}
		if(SecondaryReloadSound != null)
		{		

			_tempSound.clip = SecondaryReloadSound;
			_tempSound.Play();
		}
		
		if(SecondaryHasReloadAnimation)
		{		

			float YieldTime  = animator.GetComponent<Animation>()["Reload_Secondary"].length;
			yield return new WaitForSeconds (YieldTime);
		}


		yield return new WaitForSeconds (SecondaryManualReloadTime);
		int  HowMuchWasInTheClip  = SecondaryAmmo;
		
		if(SpareSecondaryAmmo >= SecondaryAmmoClipSize)
		{			

			SecondaryAmmo = SecondaryAmmoClipSize;
			SpareSecondaryAmmo -= SecondaryAmmoClipSize;
			SpareSecondaryAmmo += HowMuchWasInTheClip;
		}


		if(SpareSecondaryAmmo < SecondaryAmmoClipSize)
		{	


			SecondaryAmmo += SpareSecondaryAmmo;
			
			if(SecondaryAmmo > SecondaryAmmoClipSize)
			{
				SpareSecondaryAmmo = (SecondaryAmmo - SecondaryAmmoClipSize);
				SecondaryAmmo = SecondaryAmmoClipSize;
			}
			else
			{
				SpareSecondaryAmmo = 0;	
			}
		}


		if(InfiniteSecondaryMaxAmmo == true)
		{	
			SpareSecondaryAmmo = SecondaryAmmoClipSize;
		}

	
		Reloading = false;
		ReadyToFire = 1;
	}
	
	
	
	
	
	
	
	
	
	
	
	
	////////////////////////////////////////////////////////////////////
	//Playing and randomizing the Fire Sounds. No deep philosophy here.
	
	
	 void FireSound_Primary()
	{
		if(_tempSound != null && _tempSound.clip != null && _tempSound.clip != PrimaryConstantSound)
		{
			_tempSound.Stop();
			_tempSound.clip = null;
		}
		
		
		
		if(MaxPrimaryFireSounds > 0)
		{
			int  RollThePrimaryFireSound ;
			
			RollThePrimaryFireSound = Random.Range(0,MaxPrimaryFireSounds);
			
			if(RollThePrimaryFireSound == 0 && PrimaryFireSound1 != null)
			{
				_tempSound.PlayOneShot(PrimaryFireSound1);
			}
			if(RollThePrimaryFireSound == 1 && PrimaryFireSound2 != null)
			{
				_tempSound.PlayOneShot(PrimaryFireSound2);
			}
			if(RollThePrimaryFireSound == 2 && PrimaryFireSound3 != null)
			{
				_tempSound.PlayOneShot(PrimaryFireSound3);
			}
			if(RollThePrimaryFireSound == 3 && PrimaryFireSound4 != null)
			{
				_tempSound.PlayOneShot(PrimaryFireSound4);
			}
			if(RollThePrimaryFireSound == 4 && PrimaryFireSound5 != null)
			{
				_tempSound.PlayOneShot(PrimaryFireSound5);
			}
		}
	}
	
	
	 void EnvHitSound_Primary(RaycastHit hit )
	{
		int  RollThePrimaryEnviromentHitSound ;
		
		RollThePrimaryEnviromentHitSound = Random.Range(0,MaxPrimaryEnviromentHitSounds);
		
		if(RollThePrimaryEnviromentHitSound == 0 && PrimaryEnviromentHitSound1 != null)
		{
			AudioSource.PlayClipAtPoint(PrimaryEnviromentHitSound1, hit.point);
		}
		if(RollThePrimaryEnviromentHitSound == 1 && PrimaryEnviromentHitSound2 != null)
		{
			AudioSource.PlayClipAtPoint(PrimaryEnviromentHitSound2, hit.point);
		}
		if(RollThePrimaryEnviromentHitSound == 2 && PrimaryEnviromentHitSound3 != null)
		{
			AudioSource.PlayClipAtPoint(PrimaryEnviromentHitSound3, hit.point);
		}
		if(RollThePrimaryEnviromentHitSound == 3 && PrimaryEnviromentHitSound4 != null)
		{
			AudioSource.PlayClipAtPoint(PrimaryEnviromentHitSound4, hit.point);
		}
		if(RollThePrimaryEnviromentHitSound == 4 && PrimaryEnviromentHitSound5 != null)
		{
			AudioSource.PlayClipAtPoint(PrimaryEnviromentHitSound5, hit.point);
		}
	}
	
	void TargetHitSound_Primary(RaycastHit hit )
	{
		int  RollThePrimaryTargetHitSound ;
		
		RollThePrimaryTargetHitSound = Random.Range(0,MaxPrimaryTargetHitSounds);
		
		if(RollThePrimaryTargetHitSound == 0 && PrimaryTargetHitSound1 != null)
		{
			AudioSource.PlayClipAtPoint(PrimaryTargetHitSound1, hit.point);
		}
		if(RollThePrimaryTargetHitSound == 1 && PrimaryTargetHitSound2 != null)
		{
			AudioSource.PlayClipAtPoint(PrimaryTargetHitSound2, hit.point);
		}
		if(RollThePrimaryTargetHitSound == 2 && PrimaryTargetHitSound3 != null)
		{
			AudioSource.PlayClipAtPoint(PrimaryTargetHitSound3, hit.point);
		}
		if(RollThePrimaryTargetHitSound == 3 && PrimaryTargetHitSound4 != null)
		{
			AudioSource.PlayClipAtPoint(PrimaryTargetHitSound4, hit.point);
		}
		if (RollThePrimaryTargetHitSound == 4 && PrimaryTargetHitSound5 != null) 
		{
			AudioSource.PlayClipAtPoint (PrimaryTargetHitSound5, hit.point);
		}
	}
	////////////////////////////////////////////////////////////////////
	
	
	
	
	
	
	 void FireSound_Secondary()
	{

		if(_tempSound != null && _tempSound.clip != null && _tempSound.clip != SecondaryConstantSound)
		{
			_tempSound.Stop();
			_tempSound.clip = null;
		}
		
		if(MaxSecondaryFireSounds > 0)
		{
			int  RollTheSecondaryFireSound ;
			
			RollTheSecondaryFireSound = Random.Range(0,MaxSecondaryFireSounds);
			
			if(RollTheSecondaryFireSound == 0 && SecondaryFireSound1 != null)
			{
				_tempSound.PlayOneShot(SecondaryFireSound1);
			}
			if(RollTheSecondaryFireSound == 1 && SecondaryFireSound2 != null)
			{
				_tempSound.PlayOneShot(SecondaryFireSound2);
			}
			if(RollTheSecondaryFireSound == 2 && SecondaryFireSound3 != null)
			{
				_tempSound.PlayOneShot(SecondaryFireSound3);
			}
			if(RollTheSecondaryFireSound == 3 && SecondaryFireSound4 != null)
			{
				_tempSound.PlayOneShot(SecondaryFireSound4);
			}
			if(RollTheSecondaryFireSound == 4 && SecondaryFireSound5 != null)
			{
				_tempSound.PlayOneShot(SecondaryFireSound5);
			}
		}
	}
	
	 void EnvHitSound_Secondary(RaycastHit hit )
	{
		int  RollTheSecondaryEnviromentHitSound ;
		
		RollTheSecondaryEnviromentHitSound = Random.Range(0,MaxSecondaryEnviromentHitSounds);
		
		if(RollTheSecondaryEnviromentHitSound == 0 && SecondaryEnviromentHitSound1 != null)
		{
			AudioSource.PlayClipAtPoint(SecondaryEnviromentHitSound1, hit.point);
		}
		if(RollTheSecondaryEnviromentHitSound == 1 && SecondaryEnviromentHitSound2 != null)
		{
			AudioSource.PlayClipAtPoint(SecondaryEnviromentHitSound2, hit.point);
		}
		if(RollTheSecondaryEnviromentHitSound == 2 && SecondaryEnviromentHitSound3 != null)
		{
			AudioSource.PlayClipAtPoint(SecondaryEnviromentHitSound3, hit.point);
		}
		if(RollTheSecondaryEnviromentHitSound == 3 && SecondaryEnviromentHitSound4 != null)
		{
			AudioSource.PlayClipAtPoint(SecondaryEnviromentHitSound4, hit.point);
		}
		if(RollTheSecondaryEnviromentHitSound == 4 && SecondaryEnviromentHitSound5 != null)
		{
			AudioSource.PlayClipAtPoint(SecondaryEnviromentHitSound5, hit.point);
		}
	}
	
	
	
	

	 public void TargetHitSound_Secondary(RaycastHit hit )
	{	
		int  RollTheSecondaryTargetHitSound ;
		
		RollTheSecondaryTargetHitSound = Random.Range(0,MaxSecondaryTargetHitSounds);
		
		if(RollTheSecondaryTargetHitSound == 0 && SecondaryTargetHitSound1 != null)
		{
			AudioSource.PlayClipAtPoint(SecondaryTargetHitSound1, hit.point);
		}
		if(RollTheSecondaryTargetHitSound == 1 && SecondaryTargetHitSound2 != null)
		{
			AudioSource.PlayClipAtPoint(SecondaryTargetHitSound2, hit.point);
		}
		if(RollTheSecondaryTargetHitSound == 2 && SecondaryTargetHitSound3 != null)
		{
			AudioSource.PlayClipAtPoint(SecondaryTargetHitSound3, hit.point);
		}
		if(RollTheSecondaryTargetHitSound == 3 && SecondaryTargetHitSound4 != null)
		{
			AudioSource.PlayClipAtPoint(SecondaryTargetHitSound4, hit.point);
		}
		if(RollTheSecondaryTargetHitSound == 4 && SecondaryTargetHitSound5 != null)
		{
			AudioSource.PlayClipAtPoint(SecondaryTargetHitSound5, hit.point);
		}
	}
	
	
	
	
	
	
	
	 IEnumerator DryPrimaryFire()
	{
		DryFireReady = false;
		if(_tempSound != null)
		{
			_tempSound.PlayOneShot(PrimaryDryFireSound);
		}
		yield return new WaitForSeconds(PrimaryDryFireSound.length);
		DryFireReady = true;
	}
	
	
	 IEnumerator DrySecondaryFire()
	{
		DryFireReady = false;
		if(_tempSound != null)
		{
			_tempSound.PlayOneShot(SecondaryDryFireSound);
		}
		yield return new WaitForSeconds(SecondaryDryFireSound.length);
		DryFireReady = true;
	}
	
	////////////////////////////////////////////////////////////
	
	
	
	
	///////////////////////////////////////////////////////////
	//Here are the Animation  voids and the cooldown  voids. Nothing fancy - randomizing and yeilding.
	
	
	 void Primary_Fire_Animation()
	{
		if(MaxPrimaryAttackAnimations != 0 && animator != null)
		{
			int  RollTheAnim1  = Random.Range(0,MaxPrimaryAttackAnimations);
			if(RollTheAnim1 == 0)
			{
				animator.GetComponent<Animation>().Play("Fire_Primary1"); 
				
			}
			if(RollTheAnim1 == 1)
			{
				animator.GetComponent<Animation>().Play("Fire_Primary2");
				
			}
			if(RollTheAnim1 == 2)
			{
				animator.GetComponent<Animation>().Play("Fire_Primary3");
			}
			if(RollTheAnim1 == 3)
			{
				animator.GetComponent<Animation>().Play("Fire_Primary4");
			}
			
		}
		
	}
	
	 void Secondary_Fire_Animation()
	{
		if(MaxSecondaryAttackAnimations != 0 && animator != null)
		{
			int  RollTheAnim1  = Random.Range(0,MaxSecondaryAttackAnimations);
			if(RollTheAnim1 == 0)
			{
				animator.GetComponent<Animation>().Play("Fire_Secondary1");
				
			}
			if(RollTheAnim1 == 1)
			{
				animator.GetComponent<Animation>().Play("Fire_Secondary2");
				
			}
			if(RollTheAnim1 == 2)
			{
				animator.GetComponent<Animation>().Play("Fire_Secondary3");
			}
			if(RollTheAnim1 == 3)
			{
				animator.GetComponent<Animation>().Play("Fire_Secondary4");
			}
			
		}
		
	}
	
	 IEnumerator PrimaryCooldown()
	{
		
		{
			
			
			yield return new WaitForSeconds(PrimaryCooldownDelay);
			{
				BlockIdleAnimation = true;
				
				
				if(animator.GetComponent<Animation>().IsPlaying("Reload_Primary"))
				{
					yield return new WaitForSeconds(animator.GetComponent<Animation>()["Reload_Primary"].length);
				}
				
				
				if(animator.GetComponent<Animation>().IsPlaying("Reload_Secondary"))
				{
					yield return new WaitForSeconds(animator.GetComponent<Animation>()["Reload_Secondary"].length);
				}
				
				while(Input.GetButton(PrimaryFireInput) || Input.GetButton(SecondaryFireInput) ) yield return new WaitForEndOfFrame();
				
				BlockIdleAnimation = false;
				
				animator.Play("Cooldown_Primary");
				
				if(PrimaryCooldownSound != null && _tempSound != null)
				{	
					_tempSound.clip = PrimaryCooldownSound;
					_tempSound.Play();
				}
				
			}
		}
		
	}
	
	
	IEnumerator SecondaryCooldown ()
	{
		
		{
			
			
			yield return new WaitForSeconds(SecondaryCooldownDelay);
			{
				BlockIdleAnimation = true;
				
				
				if(animator.GetComponent<Animation>().IsPlaying("Reload_Secondary"))
				{
					yield return new WaitForSeconds(animator.GetComponent<Animation>()["Reload_Secondary"].length);
				}
				
				
				if(animator.GetComponent<Animation>().IsPlaying("Reload_Secondary"))
				{
					yield return new WaitForSeconds(animator.GetComponent<Animation>()["Reload_Secondary"].length);
				}
				
				while(Input.GetButton(SecondaryFireInput) || Input.GetButton(SecondaryFireInput) ) yield return new WaitForEndOfFrame();
				
				BlockIdleAnimation = false;
				
				animator.Play("Cooldown_Secondary");
				
				if(SecondaryCooldownSound != null && _tempSound != null)
				{	
					_tempSound.clip = SecondaryCooldownSound;
					_tempSound.Play();
				}
				
			}
		}
		
	}
	
	////////////////////////////////////////////
	
	
	
	
	
	
	////////////////////////////////////////////////////
	
	//Here lies the Initial Reload  void. 
	//On a second thought it seems ironic, that a starting  void was written on the very end of the code...
	
	IEnumerator InitialReload()
	{
		ReadyToFire = 0;
		if(_tempSound != null && InitialSound != null)
		{
			_tempSound.clip = InitialSound;
			_tempSound.Play();
		}
		yield return new WaitForSeconds (InitialManualReloadTime);
		ReadyToFire = 1;
	}
	
	
	
	
	
	
	
	
	
	
	

}
