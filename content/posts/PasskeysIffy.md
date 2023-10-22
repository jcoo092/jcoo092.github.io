---
Title: I'm a little iffy on Passkeys
Lead: Briefly describing my concerns with them
date: 2023-10-22
draft: false
ShowToc: false
Tags:

- Rants
- Security
    - Cybersecurity
    - Information security
    - Passkeys
    - Passwords
    - Password Managers
---

# I'm a little iffy on Passkeys

In case you haven't heard, [passkeys](https://www.passkeys.com/) are the new saviour of the security world (yes, I do say that with a tinge of sarcasm).  In fact, Google apparently just recently switched their default credential system for Gmail over to passkeys from regular-old usernames & passwords.  Passkeys are so strongly considered to be the way of the future that both 1password and BitWarden seemingly bought passkeys-focused startups so that they could add the capability to their products (I didn't manage to track down any announcements or old news articles confirming that, though).  I assume all the others in the password manager game will also try to do the same.

As I understand, passkeys basically implement public-private key systems for just about every device with some sort of hardware support for cryptographic procedures.  It seems that when setting up a new passkey, your device generates a public and private key and passes the public key to the remote authentication system.  At login time, the authenticating system uses that public key to issue a challenge, which can only be solved by someone possessing the private key.  I may have the exact workings of this wrong; I'm no expert.

Just the other day, when I had 1Password open at work, and it happened to be on the entry for my work GitHub account, it was telling me to go set up a passkey and store it in 1Password.  Given that passkeys are supposed to be specific to a given device, though, I am forced to wonder what the point of keeping it in a cloud-syncing system like 1Password is actually supposed to be.  Moreover, why can't I simply rely on the operating system's in-built support?  (This was on my work-issued device, which is still on Windows 10, so maybe there isn't such great support for passkeys there.)

I seem to recall having heard somewhere that passkeys are just an implementation of the same standard used for hardware MFA keys, but I'm uncertain how true that is.

## The Good Bits

There is indeed a lot to like about passkeys.  For one thing, they're supposed to be highly phishing-resistant.[^phishing-resistant]  In theory, they also can be managed by the OS transparently to the user.  As in, the user never sees anything happen or even needs to remember anything, but their device authenticates to the remote service automatically.  They're also random and should be cryptographically secure.  Thus, it should be pretty much impossible for anybody merely to do some sort of brute force attack and get anywhere.  Dictionary attacks and password spraying would become nearly impotent.  I presume rainbow tables would also no longer be of any use since the number of solutions that would need to be precomputed would be mind-bogglingly huge, and even then, there is likely an enormous list of possibilities that would need to be tested via brute-force since there's an extra unknown secret that complicates things.  Kind of like password salting, but perhaps on a _vastly_ greater level of effectiveness.

[^phishing-resistant]:  I'm not totally clear on how that is the case, but I presume it is because the secret – the device's private key – is never transmitted elsewhere, thus meaning that the phishers only get a valid solution to their specific challenge, not something they can re-use anywhere else.
So, all up, they should be more secure than many passwords and substantially more convenient for users in the case that everything is going well.  To the extent that users don't necessarily even notice an authentication process occurring, but it all happens appropriately in the background, nevertheless.

## The Bad Bits

First and foremost, my main issue with passkeys is that the people pushing for them seem to assume that everybody always has an up-to-date, powered smartphone with them at all times.  That will often be true for many in relatively wealthy countries, but it is not guaranteed even there.  There will still be plenty of people who can't afford them or want to choose not to use a smartphone for whatever reason.  Moreover, people lose their phones every day, never mind that batteries can run flat.  And what if, for whatever reason, you want to log into an account of yours on a device not owned and controlled by you when you don't happen to have on hand one of your devices with access already set up?

The other month, I was talking to [Yuriy Ackermann](https://github.com/herrjemand), who has long had a lot to do with the FIDO Alliance and passkeys.  He told me that some of my concerns here might not be as significant as I suspect and that the passkeys standard isn't as restricted to smartphones as it appears.  Instead, he suggested, the information security media have essentially just repeated the marketing talk from the big tech firms such as Google and Apple, who have been pretty focused on their respective smartphone systems.  I don't know if that's true, but Yuriy is (to the best of my knowledge and understanding) an expert in this area, so I'm inclined to listen carefully to him.

I also don't see that they're nearly as needed as ten or so years ago.  The whole thing about getting rid of passwords basically assumes that everyone still uses the same low-quality passwords everywhere.  While that's probably true for some, plenty don't do that now.  Password managers present another good solution to the problem.  Heck, just dreaming up passphrases and writing them down in a notebook can avoid many of the same weaknesses that passkeys try to avert.  Plus, I have the impression it's _much_ easier to reset a password than a passkey.

Another thing I'm a bit uncertain about is how passkeys appear to reduce multi-factor authentication down to single-factor.  Basically, if you have control of a device, then it seems to me that you have control of the credentials, whereas, with a password, someone still has to remember something.  If possession is equivalent to authentication, then so-called "rubber-hose cryptography" would seem to become more effective, not less.  I mean, when it's a password, you need the victim to remain conscious.  You don't need them awake or even alive if the digital key is merely something in their possession.  In theory, this is stopped by most devices requiring some approval process on the device itself, but if all that is required is a fingerprint scan (which seems to be the usual thing mentioned, along with facial recognition), then I'm unconvinced you need to keep someone conscious to exploit their device.

## In Conclusion

I could write more on this topic, but I'm too lazy right now to bother to track down the relevant sources.  Instead, I'm just writing this from my memory of reading news stories about passkeys over the past year, so take everything with a grain of salt.

When you get down to it, I suppose my concerns with passkeys aren't with passkeys themselves so much as taking password systems away and forcing people to try to use passkeys only.  While passwords certainly aren't perfect, they're pretty well-understood, we have a solution to doing them relatively well (password managers), and they have one enormous advantage over passkeys in that people _can_ memorise them.  Sometimes, knowing a secret is exactly what we want.

I'm also iffy on the dual elements that passkeys (or perhaps more, their champions) seem to expect a certain level of wealth and technical sophistication on the part of users.  Just about everyone can get their head around a passphrase, and in certain circumstances, it can be a very good thing that those can be given to someone else.  Neither of these seem to be true necessarily for passkeys.  In their rush to make things more convenient for some, it may be the case that the big tech firms are further widening the digital divide.  Not that I actually expect any of them to care.  They've made it plainly obvious many times over already that they don't.

I guess we'll have to wait and see how things shake out.  Hopefully, my scepticism proves unfounded.  I imagine there are going to end up being a few people locked out of their Google accounts, however, when something goes pear-shaped with the passkey(s) for their account and they have no way to reset things—after all, Google is notorious for not having any functional customer support from real people.
