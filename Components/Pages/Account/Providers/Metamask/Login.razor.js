// Called when the script first gets loaded on the page.
export function onLoad() {
	if (typeof window.ethereum === "undefined")
		return;

	const selectAccountButton = document.getElementById("selectAccountButton");
	if (selectAccountButton !== null) {
		selectAccountButton.style.display = "block";
		selectAccountButton.addEventListener(
			"click",
			async () => {
				const accounts = await window.ethereum.request({ method: 'eth_requestAccounts' });
				if (accounts.length == 0)
					return;

				const currentUrl = new URL(window.location.href);
				currentUrl.searchParams.set('accounts', accounts.join(','));
				window.location.href = currentUrl.toString();
			});
	}
}

// Called when an enhanced page update occurs, plus once immediately after
// the initial load.
export function onUpdate() {
}

// Called when an enhanced page update removes the script from the page.
export function onDispose() {
}
