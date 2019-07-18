/*
     * TO DO:
     * Bombs:
     *  -add lights ------------DONE
     *  -add damage to explosion ------------DONE
     *  -change how graphics look? ------------DONE
     * Chef:
     *  -add inventory ------------DONE
     *  -add ability to pick up objects ------------DONE
     *  -add ability to open objects ------------DONE
     *  -get better death sound ------------DONE
     *  -better grater sounds ------------DONE
     * Enemies:
     *  -add sprites, animations, and animation controller ------------DONE
     *  -add movement ------------DONE
     *  -make animations change according to:
     *    -direction the rat is moving ------------DONE
     *    -if the rat is attacking ------------DONE
     *    -if the rat is hurt ------------DONE
     *    -if the rat is dying ------------DONE
     *  -add health meter ------------DONE
     *  -make so they can only path toward player when they are certain distance away ------------DONE
     *  -item drops? ------------DONE
     * Scenary:
     *  -create tilemaps ------------DONE
     *  -figure out how to add collision to tilesmaps ------------DONE
     *  -figure out how to add scripted objects to tilemap (closets, exits) ------------DONE
     * GameManager
     *  -Tell enemies to stop attacking when chef is dead ------------DONE
     *  -Tell enemies to stop MOVING when chef is dead ------------DONE
     *  -modify the "entities" sorting layer order based on the y axis. ------------DONE
     *  -Play sound when win conditioned fulfilled!!!
     * Effects:
     *  -lighting ------------DONE
     *    -eating more carrots makes your vision bigger? ------------DONE
     *    -shadows? ------------DONE
     *  -camera shake ------------DONE
     *  -MUSIC
     * UI:
     *  -energy bar ------------DONE
     *  -health bar ------------DONE
     *  -scene transitions!
     *      - ADD IN FAST FADE TO BLACK WHEN ENTERING LEVELS ------------DONE
     *      - SLOW WHEN LEAVING AND DYING, ALSO STOP ENEMY MOVEMENT ------------DONE
     *  -scenes!
     *    -gamemanager has needed things I guess
     *  -inventory ------------DONE
     *  -lock camera to within bounds ------------DONE
     *  -messages under certain circumstances - need keys, have no weapons, etc. ------------DONE
     *      -HOOK THESE UP ------------DONE
     * Bosses:
     *  -lol all of that again^ ------------DONE
     *  -have bigger healthbar on top of screen for bosses ------------DONE
     * Levels:
     *  -recreate original 5 level layouts
     *  - DEATH SCREEN------------------------------------------------------------DONE
     *   -level 1 - kitchen------------------------------------------------------------DONE
     *      - must kill all rats, find key
     *   -level 2 - maze basement------------------------------------------------------------DONE
     *      - make your way to exit
     *      - introduce bombs at very end
     *   -level 3 - first boss------------------------------------------------------------DONE
     *      - kill him to reveal exit
     *   -level 4 - sewers------------------------------------------------------------DONE
     *      - smaller level
     *      - close all the red pipes, then kill all the rats
     *      - introduce green exploding rats if have time
     *      - have graters in side room
     *   -level 5 - small maze sewer------------------------------------------------------------DONE
     *      - knives in side room
     *      - need to find key then find way out
     *   -level 6 - long corridor (final boss palette with water------------------------------------------------------------DONE
     *      - either continuous stream of enemies spawning at end of tunnel
     *      - or knife throwing rats accross river
     *   -level 7 - final boss------------------------------------------------------------DONE
     *      - roughly same behavior
     *      - maybe have a ground smash ability?
     *      - idk
     *  -add proper functionality to levels
     *    -enemies ------------DONE
     *    -win conditions ------------DONE
     *    -BOSSES ------------DONE
     *    -item drops ------------DONE
     * Bugs:
     *  -if chef is moving, and he dies, hurt animation and sound plays, with death sound, but no death animation ------------DONE
     *  -rat not playing hurt animations ------------DONE
     *  -Ability to push rats while they are hurt! Remove this! If you just remove the colliders then the rats will fall into walls ------------DONE
     *  -Enemies can't path around eachother, get stuck in Chef's original position? ------------DONE
     *  -Add info about items when first pick them up! Make it so you don't get same message on same playthrough (like you died or something) ------------DONE
     *  -Play special sound when pick up something for the first time! ------------DONE
     *  -previous bugfix that stopped player from pushing hurt enemies DID NOT WORK. the fix affected ALL enemies, not just that specific one. ------------DONE
     *  -Not bug, but entitymanager is never used in Health...
     *  -Rat02 death sound gets cut off
     *  -no_energy message does not work ------------DONE
     *  -multiple messages play at once ------------DONE
     *  -Don't use playermovement or playercombat EVER in playermanager... remove references?
     *  -a lot of my coroutines could just be animations...
     *  -player can die multiple times. need to set deathcoroutine to only play once
     * Stretch goals:
     *  -NEW LEVELS:
     *    -minibosses?
     *    -survival, need to last long enough for door to open against constant stream of enemies?
     *  -new enemies
     *    -exploding rats that need to be killed with knives
     *      -glow green?
     *    -rats that throw knives!
     *    -mini purple bosses that use purple and orange attacks and walk on top of levels
     *  -2 player deathmatch (hard as hell)
     *  
     *        
     *        TO DO:
     *        - playtest
     *        -pause menu causes crash on build.
     *        
     */
