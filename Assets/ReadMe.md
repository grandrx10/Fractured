Code docs

# Card Base:

## Behaviors:
At the core of the card system is the Behavior class. Behaviors define all the interactions that a card has. Each
behavior is a ScriptableObject, and cards can contain any number of behaviors, aside from certain default behaviors
(explained later)

### Design:
The Behavior system supports both composition and inheritance. The main abstraction layer for behaviors are Behavior
Tags (_Cards\Core\BehaviorTags_). Behavior Tags are interfaces that can be filtered by the Card.

#### Implementation:
To create a behavior that implements an existing Tag, implement the interface's methods.

To use behaviors with a certain Tag, see Cards.

#### Defaults:
The Card class defines default Behaviors, such as health and use. These are automatically applied to the card at
initialization if there are no other classes implementing the relevant interface. It can be assumed that these will
be on every Card.

#### Additional Notes:
The **HasStateTag** is used to mark when a Behavior stores a local state; that is, we must clone the Behavior at
initialization so different cards don't share the state.

It is recommended to use interfaces for certain Behavior patterns, and inheritance when Behaviors perform similar actions.

#### UI:
Behaviors have an optional display text feature. To use this, you can override either **GetDescription** or
**GetMenuObject** (or both). **GetDescription** shows a string directly on the card. **GetMenuObject** shows any UI
element under the card when in the inventory.

## Cards and Agents:
The Card class is just a container for Behaviors with some utility functions, UI, and states.

Likewise, an Agent is just a container for Cards with some utility functions.
### Important Features:
#### Hand and Deck:
The hand is the active set of cards that can be used. The deck is the rest of the cards that the Agent owns. For example,
if a minigame has one input (represented by a card), we would want to move the player agent's hand to the deck before the game,
and put it back after.
#### Querying Behaviors:
Cards implement **TryGetBehavior\<T>** and **GetAllBehaviors\<T>**, which can be used to find relevant behaviors.
#### Defaults:
To define a default behavior for all Cards, three things must be supplied:  
- A Type of class (can be an interface), for which if no Behaviors implement it, the default will be substituted.
- A Type that inherits Behavior (the default type).
- A path to a ScriptableObject (relative to a Resources folder) that has the default type.
### Agents:
Agents are meant to be abstract agents that only interact by providing Cards. The active environment deals with all
other interactions. There are two important actions for Agents:

#### SelectCardAsync(Func<Card, CardSubmitState> callback, int requiredCards)
Request a card from the Agent. The Agent can submit the card at any time, given the callback is not canceled (by
**CancelSelection**). The _requiredCards_ field specifies how many cards are required before removing the callback. There 
is only support for one async card request at a time, more will throw an error.  

The callback can also specify a success value; only a success will count as a card being submitted.

#### SelectCardImmediate()
Request a card from the Agent. The Agent must immediately submit a card. Defaults to a random card in their hand.

## Environments:
An Environment is a system that interacts with Agents. See Physical Environments for implementation details for environments
with physics.

It is the responsibility of the environment to manage temporary state values such as health and mana. Likewise,
any associated UI for these values should be attached per-environment.

An agent should only be attached to one environment at a time.

For multi-agent environments, _CallbackWaiter_ is probably helpful (see below).

## Physical Environments:
### Use Behavior:
The **Use** behavior is the main interaction point for physical environments. To define card behavior like throwing,
implement **IBehaviorUseListener**.
### PhysicalObject:
The **DefaultUseBehavior** supports a basic card throwing animation, with an extensible **PhysicalObject** class.

PhysicalObjects implement two events: _Init_ and _Move_. (usually, it is recommended to use _Move_)

The default physical card has two **PhysicalObject** listeners, _CurveTowards_ (_Init_ listener) and _LinearThrow_ (_Move_ listener).

#### Init:
Executes at fire time. Used by the default physical card behavior (_CurveTowards_). Use this if you want to spawn an
object and also replace _CurveTowards_.

#### Move:
Executes when called. By default, _CurveTowards_ calls this when the card is finished the curving animation. Use this to,
for example, create a projectile from the card after it has been aligned to the target direction.

# Other:
## Code Utils:

### GlobalState:
A singleton that stores variables about the game state. Supports 4 data types:

**Event**: A single name. Can be treated as a boolean.

**String**: A name and an associated string.

**Integer**: A name and an associated int.

**Tuple**: A name and an associated list of ints.
Elements can also be added set-wise (duplicate-free).

### ArticleHelper:
A class that adds the appropriate _a_ or _an_ before a noun.

### Delay:
Use to delay a function call by a certain number of seconds. Supports two overloads:  
**Call(MonoBehavior owner, float time, Action callback)**  
Fires if the attached object is not destroyed.

**Call(float time, Action callback)**  
Will always fire; ensure no references are destroyed before it fires.

### CallbackWaiter:
A wrapper class for awaiting multiple callbacks. Has a pair type and an N-tuple type:

#### CallbackWaiter2\<T>(Action<T, T> onDone)
Calls _onDone_ when both _A_ and _B_ are set.
#### CallbackWaiterN\<T>(int count, Action<T[]> onDone)
Calls _onDone_ when both count values are provided.

## Other Utils:

### FloatBar (GameObject):
A UI bar that displays any float field as a slider.

### ProBuilder (Package):
The ProBuilder Package is installed for quick modeling in Unity.

### LeanTween (Package):
The LeanTween Package is installed for animating interpolation easily.

## Custom Editor:

### [PrefabComponent]
Use on ScriptableObject fields that need to reference prefabs with a certain Component. Adds an object picker that
actually shows the relevant objects.
