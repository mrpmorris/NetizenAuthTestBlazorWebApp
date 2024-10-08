// Called when the script first gets loaded on the page.
export function onLoad() {
	const hiddenForm = document.getElementById("redirectForm");

	if (hiddenForm) {
		hiddenForm.submit();
		return;
	}

	const hasEthereum = (typeof window.ethereum !== "undefined");
	const accountInput = document.getElementById("accountInput");
	const payloadInput = document.getElementById("payloadInput");
	const signatureInput = document.getElementById("signatureInput");
	const signInButton = document.getElementById("signInButton");
	const copyPayloadButton = document.getElementById("copyPayloadButton");
	const pasteSignatureButton = document.getElementById("pasteSignatureButton");

	const displaySelector = hasEthereum ? '.metamask-sign' : '.manual-sign';
	document.querySelectorAll(displaySelector).forEach(element => {
		element.style.display = 'block';
	});

	copyPayloadButton.addEventListener(
		"click",
		async (event) => {
			event.preventDefault();

			const payload = payloadInput.value;
			await navigator.clipboard.writeText(payload);
			alert('Payload copied to clipboard.');
		}
	);

	pasteSignatureButton.addEventListener(
		"click",
		async (event) => {
			event.preventDefault();
			const clipboardContents = await navigator.clipboard.readText();
			signatureInput.value = clipboardContents;
		}
	);

	signPayloadButton.addEventListener(
		"click",
		async (event) => {
			event.preventDefault();

			const payload = payloadInput.value;
			const account = accountInput.value;

			try {
				const signature = await ethereum.request({
					method: 'personal_sign',
					params: [payload, account]
				});

				signatureInput.value = signature;
			}
			catch (error) {
				console.error('Signing failed', error);

				if (error.code === 4001)
					// User rejected the request
					alert('Signature request was rejected by the user.');
				else
					alert('An error occurred during the signing process.');
			}
			signInButton.form.submit();
		}
	);
}

// Called when an enhanced page update occurs, plus once immediately after
// the initial load.
export function onUpdate() {
}

// Called when an enhanced page update removes the script from the page.
export function onDispose() {
}
