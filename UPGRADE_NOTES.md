# Cat on a Wire – upgraded to Unity 6 (6000.5.10f1), 2026-10-16

A game-jam game (2014), built on the source code of another game. Source: the SVN checkout
`C:\Users\razva\projects\bipolar-bear\trunk\UGJ` (revision 56, November 2014, from
`theotterside.svn.cloudforge.com/ubigamejam`; the scenes were saved with Unity 4.3–4.5). That checkout keeps its
old folder name and was not touched; this folder is a copy without its old `Library` folder and the `*.bak` files.

## What was changed
- **Name**: product name "Cat on a Wire" (was "UGJ"), Windows id `com.DefaultCompany.CatOnAWire`, menu title,
  build `Builds/Windows/CatOnAWire.exe`, editor menu **Cat on a Wire**.
- **Scripts** (Unity 4 APIs that Unity 6 removed):
  - `rigidbody2D.` → the `rigidBody2D` field; `.velocity` → `.linearVelocity` (CharacterController2D).
  - `BoxCollider2D.center` → `.offset` (CharacterController2D).
  - `Application.LoadLevel` → `SceneManager.LoadScene` (GameLogic, PlayerController, MenuLogic).
  - `particleSystem.renderer` → `GetComponent<ParticleSystemRenderer>()` (SetParticleSortingLayer).
  - `FindObjectOfType` → `FindAnyObjectByType` (GameLogic).
- **GUITexture / GUIText** no longer exist. Their settings (texture, colour, pixel inset, text, font, size, offset)
  were read out of the original binary scenes and put back on the same objects as `LegacyGUITexture` /
  `LegacyGUIText` (Assets/Scripts), which draw the same way: object position = point on screen (0..1), pixel values
  from there, y from the bottom, 0.5 grey = unchanged colour, higher z on top. The game was laid out for 1280 x 720;
  those pixels are now scaled to the window, so the layout holds at any resolution.
- **MenuLogic** uses `LegacyGUITexture.HitTest` like before. Because the menu pictures are missing (see below), it
  draws a plain PLAY button and the title "Cat on a Wire" in the same place when the button has no texture.
- **Balance bar** (`BalanceBehavior`, on BalanceCursor), rewritten so it works. In 2014 it threw a
  NullReferenceException at every level start (`bar`, `healthyBar`, `sickBar` never filled in) and its range check
  (2..10 in world units) did not match the cursor, which is drawn on the screen. Now: the cursor drifts, with a new
  random drift every 2 s (was every 120 frames); P / L push it back; halfway out the frame flashes red; at the end of
  the frame (125 px from the middle) the cat falls and the level restarts. Drift and push are the 2014 numbers
  (random -10..10, push 5, x 0.15 over a half range of 4 units), now per second instead of per frame.
- **Falling through a gap** in the ground restarts the level (`PlayerController.fallLimit`, -30; the lowest ground
  is at about -10). In 2014 the cat fell for ever.
- **FrameRate.cs**: held at 60 fps. Clouds, parallax and enemies move a fixed step per frame, so on a 120/144 Hz
  screen the game would otherwise run 2–2.4x too fast.
- A disabled script that was already missing in 2014 (fields playerSpeed, autoRun, theAnimator…, no file anywhere
  in the checkout) was removed from the Cat.
- All assets are now saved as text (Force Text), so scenes and prefabs can be compared and merged.
- **Textures compressed**: 20 of 24 now BC7 (high quality, 4x smaller than uncompressed). Asfalt (1920 x 498),
  brickWall (425 x 600), Duck (477 x 257) and Rope (400 x 10) stay uncompressed: BC7 needs sizes divisible by 4.
  Build: 167 MB (was 382 MB).
- **Unused assets removed** (nothing in the two scenes used them, directly or through a prefab, animation, animator
  controller or material, and no code refers to the scripts): Buton_slider, Catnip, pole, Sarma, Slider_alb,
  Slider_rosu, SOARECI, spinningCoin, spinningCoin2, stalp_1, stalp_3, stalp_6 (png); Sprites/Background.jpg,
  Sprites/CatCrouch.png; Cat.prefab and Main Camera.prefab (top level); Prefabs/Cat, QTE_Prompt, Snow_Jam,
  Snow_ParticleSystem_All and the Prefabs/HUD folder; Materials/LightRay.mat; the Standard Assets folder; scripts
  CloudBehaviour, DukeLogic, GrowlBehavior, HUDController.
- Removed default packages the game does not use: analytics, purchasing, collab-proxy (Unity Version Control),
  multiplayer center, XR legacy input helpers, AI navigation, timeline.
- `Assets/Editor/GameBuild.cs`: menu **Cat on a Wire > Build Windows** (also sets the scene order: menu = 0, level = 1).
- `Assets/Scripts/GameTestRun.cs`: only active when the game is started with `-cat-test <folder> [idle|bot|fall]`;
  does nothing in normal play. Screenshots, logs the cat's position, balance, level reloads and errors, then quits.
  idle / bot / fall make the obstacles harmless for the test: idle = nobody presses P / L (the cat should fall off
  balance), bot = P / L pressed when the cursor is a third of the way out (the cat must stay up), fall = bot, with
  the fall check on (the level must restart at the first gap).

## Test results (1280 x 720 window), 0 errors in all four runs
- normal: menu, then the level; with no key presses the cat runs into the first pole after 1.7 s and the level restarts.
- idle: balance drifted out and the cat fell after 14.7 s (one run); the drift is random, another run reached 0.73 in 20 s.
- bot: balance stayed between -0.24 and 0.24 for 20 s; no fall.
- fall: the cat dropped through the first gap; the level restarted after 3.2 s, every time.

## Missing in the repository (not caused by the upgrade)
Revision 56 is a snapshot from during the jam; these were never committed and are not in `Library` either:
- The menu background and the Play button pictures (pressed / unpressed).
- The in-level GUI pictures: TimerIcon, Left/Right/Jump/Action buttons, ExtraIndicator(+Text), StateIndicator(+Text),
  PillIcon. Their whole `GUI` group is switched off in the scene anyway.
- All sounds (jump, pill, punch, growl, step). The player calls them; nothing plays.
If you find the art, drop the files in and set them on the objects (LegacyGUITexture > Texture; MenuLogic >
Play Button Unpressed / Pressed); the positions and sizes are already right.

## Left as it was
- `QTE_Prompt`, a 3D text "Text" placeholder in the level, is active in the scene.
- The level restarts at a checkpoint when the cat touches anything tagged Damage / Destroyable (e.g. the poles).

## Controls (from PlayerController)
The cat runs by itself. W jump, S crouch, A / D left / right, Space fire / action, Enter consume, Left Ctrl secondary
action, P / L balance up / down, Esc back to the menu (Esc on the menu quits).
