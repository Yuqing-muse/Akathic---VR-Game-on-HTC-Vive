# Akathic---VR-Game-on-HTC-Vive

# Introduction
The VR game is a RPG 3D game based on HTC Vive, using unity and 3Dmax. Our team mainly uses C#. All models and animations are created by our own. 
The game includes a backpack system, a combat system, a magic system, a UI system, an animation system, an equipment collection system and more. The main character can switch equipment to perform magical attacks or physical attacks. In the magic system, I have implemented gesture recognition and sound recognition. By recognising the shape of the handle drawn in the air to perform different magical attacks, such as ice, fire, laser, and magical enchantments that boost one's attributes.

# Demo
**90 degree highland immersion**
![90度高地沉浸感](https://user-images.githubusercontent.com/62585203/131242199-e3ad7a38-49f8-4737-8ab4-abd18d991bd9.png)
**Skill attack effect**
![QTE](https://user-images.githubusercontent.com/62585203/131242216-b040b79e-a5a4-4749-a7bc-dc7e63dca371.png)
**Backpack**
![背包](https://user-images.githubusercontent.com/62585203/131242226-b8d121af-ec94-414f-81f1-661ba40579de.png)
**Scene interaction**
![场景交互](https://user-images.githubusercontent.com/62585203/131242231-6cdb31f4-0fbf-414d-8be8-d538883675bd.png)
**Collect equipment**
![捡起武器](https://user-images.githubusercontent.com/62585203/131242237-77851982-0259-4cf1-b16c-3b6770e698bc.png)
**Gesture recognition**
![手势识别](https://user-images.githubusercontent.com/62585203/131242251-ffbdad30-faf0-43ac-932f-bf9fb3918db7.png)
**Equipment**
![武器](https://user-images.githubusercontent.com/62585203/131242263-d0599cb1-0a82-4d12-8024-d574bc085187.png)
**Weapon skill attacks**
![武器技](https://user-images.githubusercontent.com/62585203/131242274-dc98d8e2-8037-4506-91c5-bee686a664ce.png)



# Fighting Experience
The VR scene is like a computer's steam, i.e. console type gaming scene, requiring a high level of concentration, immersion and manipulation, with a 'forward leaning' gaming model. Rather than a "backward leaning" gaming environment like the lounge scene of tablets and mobile phones. I would expect this to be a hardcore, fighting oriented role-playing game (ARPG) with high fighting requirements. However, the current VR environment has some limitations for action-based games. For example, suppose the player uses the joystick to control movement. In that case, unless it is instantaneous, there will be a degree of dizziness caused by the mismatch between the body and the vision. And the way our team currently operates is limited by the Vive handle, which can be quite limiting for a more realistic operating experience of gameplay. It's also complicated to recreate the excitement of an action game. Because of the first-person perspective of a VR game, there is a huge amount of visual information to process within the perspective, which can be a massive headache for players new to gaming and VR games to continue playing. The following optimisations have been made in the game to improve the combat experience.

**Critical Area Attacks**: Limit the area where certain types of monsters need to be attacked to improve the handling requirements and playability, while the rest of the area can be judged as having no or low damage. (The following are all about critical areas) Here's an idea, if there are large monsters then you can use terrain and jumping to make a mounted attack.
i. CumulationDamage: When the damage to the part exceeds a certain value, it will cause the part to break, and the monster will be hardened, and then enter another state, which can be a rage state to increase the difficulty or a weak state to decrease the difficulty, in short, switch to a different state, action, and move performance, depending on the different monster settings.
ii. Armour Bias: The armour of this part of the body may exhibit different phasing to different types of weapon attacks, for example sword-like slashing types and blunt heavy types have different damage reductions when calculating damage, as well as attribute damage granted to the weapon by props and secret spells.
iii. Otk Execution Effect: When a monster reaches a certain level of blood, it will be executed by a single blow to the head or a specific location.

**Weapon type and damage calculation**
**Striking and bouncing back**
i. The monsters only have armour resistance and relatively regular AI for moves and dodging, and do not fight. Combined with the limitation of attacking parts, this only requires the player to run and attack by observing the monsters' behaviour, without considering how the monsters will block, and without considering the feedback of the monsters' blocking to the player's action of slashing with the handle, which can appropriately reduce the complexity of the game.
If AW>W*e, the monster will fail to block and be knocked back a distance, else it will succeed without damage (feedback to the joystick vibration). If AW<W*V*f, the counter will succeed and the monster will be knocked back, else it will fail and take half as much damage.

**Dodging**: It is best to be able to do this by detecting helmet displacement, otherwise displacement controlled via the handle tends to bring on dizziness.
**Magic**: the left handle takes out the magic guide book from the waist, and the right handle button gestures simple geometric shapes on the UI hovering in front of you to detect and cast secret spells. Tentative secret spells include weapon attribute enchantment, self-recovery, flash and other monster-holding spells.

