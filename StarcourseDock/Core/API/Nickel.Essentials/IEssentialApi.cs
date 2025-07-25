namespace Nickel.Essentials;

/// <summary>
/// Provides access to <c>Nickel.Essentials</c> APIs.
/// </summary>
public interface IEssentialsApi
{
	/// <summary>
	/// Registers a new hook related to the features of the <c>Nickel.Essentials</c> built-in mod.
	/// </summary>
	/// <param name="hook">The hook.</param>
	/// <param name="priority">The priority for the hook. Higher priority hooks are called before lower priority ones. Defaults to <c>0</c></param>
	void RegisterHook(IHook hook, double priority = 0);
			
	/// <summary>
	/// Unregisters the given hook related to the features of the <c>Nickel.Essentials</c> built-in mod.
	/// </summary>
	/// <param name="hook">The hook.</param>
	void UnregisterHook(IHook hook);
	
	/// <summary>
	/// Returns the EXE card type (see <a href="https://cobaltcore.wiki.gg/wiki/CAT">CAT</a>) for the given <see cref="Deck"/>.<br/>
	/// Takes into account EXE cards added by legacy mods, which are not available by reading <see cref="PlayableCharacterConfigurationV2.ExeCardType"/>.
	/// </summary>
	/// <param name="deck">The deck.</param>
	/// <returns>The EXE card type for the given <see cref="Deck"/>, or <c>null</c> if it does not have one assigned.</returns>
	Type? GetExeCardTypeForDeck(Deck deck);
	
	/// <summary>
	/// Returns the <see cref="Deck"/> for the given EXE card type (see <a href="https://cobaltcore.wiki.gg/wiki/CAT">CAT</a>), if the type represents such a card.
	/// </summary>
	/// <param name="type">The EXE card type.</param>
	/// <returns>The <see cref="Deck"/> for the given EXE card type, or <c>null</c> if the type does not represent such a card.</returns>
	Deck? GetDeckForExeCardType(Type type);
	
	/// <summary>
	/// Checks whether the given type represents an EXE card type (see <a href="https://cobaltcore.wiki.gg/wiki/CAT">CAT</a>).
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>Whether the given type represents an EXE card type.</returns>
	bool IsExeCardType(Type type);

	/// <summary>
	/// Checks whether an EXE card for the given deck is currently blacklisted from being chosen as a starter card.
	/// </summary>
	/// <param name="deck">The deck to check for.</param>
	/// <returns>Whether an EXE card for the given deck is currently blacklisted.</returns>
	bool IsBlacklistedExeStarter(Deck deck);

	/// <summary>
	/// Checks whether an EXE card for the given deck is currently blacklisted from being offered during a run.
	/// </summary>
	/// <param name="deck">The deck to check for.</param>
	/// <returns>Whether an EXE card for the given deck is currently blacklisted.</returns>
	bool IsBlacklistedExeOffering(Deck deck);

	/// <summary>The <see cref="UK"/> part of a <see cref="UIKey"/> used by the button that toggles between showing a list of ships on the <see cref="NewRunOptions"/> screen.</summary>
	UK ShipSelectionToggleUiKey { get; }

	/// <summary>The <see cref="UK"/> part of a <see cref="UIKey"/> used by each button on the list of ships on the <see cref="NewRunOptions"/> screen. The <see cref="UIKey.str"/> will be set to the ship's key.</summary>
	UK ShipSelectionUiKey { get; }
	
	/// <summary>Whether the <see cref="NewRunOptions"/> screen is currently showing the list of ships instead of a grid of characters.</summary>
	bool IsShowingShips { get; }
	
	/// <summary>The ship that is currently being previewed by hovering over its button while <see cref="IsShowingShips"/> is <c>true</c>.</summary>
	StarterShip? PreviewingShip { get; }

	/// <summary>
	/// A hook related to the features of the <c>Nickel.Essentials</c> built-in mod.
	/// </summary>
	public interface IHook
	{
		/// <summary>
		/// Allows controlling whether the "Order" sort mode is enabled for the given <see cref="CardBrowse"/>.
		/// </summary>
		/// <param name="args">The arguments for the hook method.</param>
		/// <returns><c>true</c> if the "Order" sort mode should be enabled, <c>false</c> if not, <c>null</c> if the hook does not care.</returns>
		bool? ShouldAllowOrderSortModeInCardBrowse(IShouldAllowOrderSortModeInCardBrowseArgs args) => null;
		
		/// <summary>
		/// The arguments for the <see cref="ShouldAllowOrderSortModeInCardBrowse"/> hook method.
		/// </summary>
		public interface IShouldAllowOrderSortModeInCardBrowseArgs
		{
			/// <summary>
			/// The route the "Order" sort mode should be enabled for or not.
			/// </summary>
			CardBrowse Route { get; }
		}
	}
}