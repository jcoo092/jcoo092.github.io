---
Title: Deploying Then Securing the OWASP Juice Shop, Part Six of ?
Lead: Penetration Testing Amateur Hour
date: 2023-12-24
draft: true
Tags:

  - AppSec
  - Hacking
  - OWASP
  - OWASP Juice Shop
  - Penetration Testing
  - Security

Series:
  - Deploying Then Securing the OWASP Juice Shop
---

# Penetration Testing: Amateur Hour

In this post, I am essentially going to fire up the OWASP Juice Shop (OJS) locally, navigate to the scoreboard to see the intended challenges, and then have a go at solving as many as I think I have a hope in heck of achieving.  Given that I am not a penetration tester (in fact, I think I'd probably be rubbish as a professional pentester if I attempted it), I don't expect to solve all that many of the challenges, at least not without getting some significant hints from elsewhere.  We'll see how I go, though.

This post is likely very much just going to be a long, rambling, lightly edited series of notes to myself as I go through trying to solve the challenges.  Hopefully, that stream-of-consciousness style will actually be of help to someone, though.  With any luck, I'll end up including the missing piece of the puzzle for someone else looking at one of the challenges, and enable them to go solve it themselves.   **Be warned, though:  I'm not going to attempt to redact or hide things in this post, so there will probably be a bunch of spoilers littered throughout.**

## Wait, what happened to parts three, four and five?

When I started on this blog series, I intended to spend a few posts exploring different ways to deploy the OJS to AWS.  I did indeed do a bit of that in [post two](../two).  It turns out, however, that this was both more complicated and fiddly than I had anticipated, and moreover that it is actually really interminably dull to me.  Cloud deployments are, of course, a very important aspect of modern software development, but they're not something which excites me in the slightest.  Rather, it's the application security aspects of all this that I'm interested in.

Given all this, and that I'm currently working at a job where the cloud deployment stuff is largely all handled for me (and thus I don't _need_ to learn all about it right now), I have decided to skip trying to finish the deployment posts for the time being.  I'm certainly not ruling out going back to it, but I don't anticipate working on that any time in the near future.  Thus, this post.  That still doesn't explain the lack of part five, though.  For part five, I intended to model threats for the OJS, and then use that to help inform the later posts in this series.  After all, a _lot_ of people who know what they're talking about suggest that threat modelling is an excellent first step.  E.g. that was the key message of [the article](https://doi.org/10.1145/3608965) "Coming of Age" by Stefano Zanero, that appeared in the September 2023 edition of the Communications of the ACM.

The problem is that I have somehow managed to go years in learning about appsec while only ever developing the most rudimentary understanding of threat modelling.  It'll take me some proper background learning to be able to produce anything worthwhile for this blog series, and I'm keen to get (re)started on it.  Thus, I'm moving straight to fumbling around poking at a local instance, to see which of the challenges I can solve based on what I currently know (and the tools I have installed on my home computer).

## First Impressions

As I said in one of the earlier posts, I managed to find the score board myself.  Honestly, there wasn't anything smart about it.  I just guessed that the URL would probably be something like `domain/scoreboard`, and tried a variation or two on that.  I probably should have taken the first piece of advice of the introductory tutorial, though, which said

> Look through the client-side JavaScript in the Sources tab for clues. Or just start URL guessing. It's up to you!

It turns out that, most of the way through the main.js file, they list all the paths for the pages on the domain.  The name of the relevant JS component is obfuscated, but, of course, they can't actually obfuscate the name of the path without breaking things.

{{< figure src="score-board_path_in_source.png" title="The path to the score board, right there in the source code." alt="A screenshot of some of the OWASP Juice Shop's source code, showing that the path to the score board is hard-coded in client-side Javscript." >}}

The score board lists all sorts of challenges, with an estimated difficulty rating for each.  For the time being, I'll focus only on challenges rated from 1-3, as higher challenges are either potentially beyond my skills and/or will take significantly more effort than I can be bothered with right now to achieve.  I'll include all the categories, though, since I don't think there's any one particular thing I'm especially good at.

Besides the challenge of finding the scoreboard itself, a handful of other challenges jump out at me as ones I might be able to achieve.  Especially some of the ones that sound like they probably are about bypassing client-side validation.  Near the bottom, however, one jumps out at me as definitely a good place to start:  "Security Policy".  It's description reads

> Behave like any "white-hat" should before getting into the action.

I'm not 100% sure what they're talking about, but it seems like a good idea to investigate this first.  The mouseover tip says something about reading the security policy, so they're _probably_ talking about making sure you understand what's considered in and out-of scope for the testing.  I'll click on the extra hints button to find out more.  That opens a link to what I think is probably the official walkthrough for the challenge.  Unfortunately, it looks like they have re-jigged the URLs on the website since v15 of OJS was released, so it gives me a missing page message.  Ah well, it's pretty easy to find that page again by just going back to the base domain and starting from there.

The write-up just has to this say

> This challenge asks you to act like an ethical hacker.  As one of the good guys, would you just start attacking an application without consent of the owner?  You also might want to read the security policy or any bug bounty program that is in place.

So yeah, they were talking about the usual practice of getting explicit permission and defining scopes, etc.  Of course, in this instance, the documentation around the OJS more or less has already given explicit permission (plus I'm only running the system locally in a Docker container on my own computer, so yes I can grant myself permission to attack that system).  I'm dimly aware of the [security.txt](https://securitytxt.org/) standard, so I'll give that a try.  My first attempt, http://localhost:3000/#/.well-known/security.txt, doesn't seem to do anything.  Maybe they don't have one?  Trying it without the `.well-known` part doesn't seem to work either.

I move on to try to search through the code again, much as with the score-board.  No luck there.  I also crack open the privacy policy (getting that solution ticked off in the process), just in case it is linked in there, but no.  At this point, I'm a bit stumped about all this.  Ordinarly, I wouldn't _nearly_ yet revert to checking the answers, but since this one is all about engaging in good practices up-front, I'll make an exception.  The [solution](https://pwning.owasp-juice.shop/companion-guide/latest/appendix/solutions.html#_behave_like_any_white_hat_should_before_getting_into_the_action) seems to say the same thing as what I tried, though.  Except, I eventually notice that in my original attempt there was a `#` in there, but there isn't in the listed solution.  Changing to http://localhost:3000/.well-known/security.txt does indeed solve the challenge.  There's possibly some relevant info in there, so I'll keep that tab open for now.

That's three down, a whole bunch more to go.

## Some Basic Improper Input Validation

### Zero Stars Given

Looking through the (filtered) list of challenges, some of the ones that catch my eye are from the 'Improper Input Validation' set.  Maybe because I have a little experience of doing this sort of thing in the SANS Holiday Hack Challenge, I feel like I could make a stab at it.  E.g., the challenge 'Zero Stars', which says "Give a devastating zero-star feedback to the store."  My initial thought for this is that probably there is some sort of validation on the feedback form to prevent someone submitting a zero rating, but anybody who bypasses the GUI is unstopped.  I.e., there's front-end validation, but no back-end validation.  Turns out the front-end uses a slider to set a score between 1 and 5.

{{< figure src="customer_feedback_form_empty.png" title="The customer feedback form when you first open it." alt="A screenshot of the OWASP Juice Shop's customer feedback form, as it is when you first navigate to the page." >}}

The easiest way I know of to try out fiddling with such things is to submit a valid request, and then use the browser's built-in development and debugging tools to replay the request with an updated payload.  So, F12 it is, to crack open Firefox's dev tools.  I fill out the feedback form with some dummy info and a 1-star rating, and watch the network requests.  I notice that, besides the POST request to the `/api/Feedbacks` endpoint, there are also GET requests to `whoami` and `/rest/captcha/`.  I presume the latter validates that the answer provided for the captcha challenge is correct, but I'm not sure what the other one is about.  They could well be interesting later on, but I don't _think_ I need them right now to achieve the challenge.  I also notice that I get back as part of the response the ID 8.  So there's probably seven other earlier feedback items in the database.

I do wonder at this point, though, whether I can do a GET request to the Feedbacks endpoint mentioned earlier.  I try just requesting it via cURL, but get an error an invalid token.  Presumably, I need to include some header or another for the endpoint to accept it.  I'll try again via the dev tools, but first the zero-star rating (just in case I muck something up).  I update the rating entry in the replayed request's body to 0 and hit send.  Success!  The ID on the response to this one is 9, suggesting that the feedback items are indeed stored with sequential IDs.

### While I'm Here

Looking through the list of challenges for others that I can probably do while I'm here, I notice 'CAPTCHA Bypass', which is about submitting 10 or more instances of feedback within 20 seconds.  Given that I could already replay the initial submission to do it once, if I can just repeat that same request in a for loop in a script, that should be trivial.  I also discover that Firefox very helpfully actually provides an option to put the correct cURL command to repeat the request onto the clipboard.  So, between that and a sliver of Powershell, that challenge is also already solved.  The precise command used was:

```powershell
1..10 | % { curl "http://localhost:3000/api/Feedbacks/" -X POST -H "User-Agent: Mozilla/5.0 (Windows NT 10.0; rv:121.0) Gecko/20100101 Firefox/121.0" -H "Accept: application/json, text/plain, */*" -H "Accept-Language: en-GB,en;q=0.5" -H "Accept-Encoding: gzip, deflate, br" -H "Referer: http://localhost:3000/" -H "Origin: http://localhost:3000" -H "DNT: 1" -H "Connection: keep-alive" -H "Cookie: language=en; welcomebanner_status=dismiss; continueCode=xmOK5wX8WKPpQY6Mr4neoLglBOAP9LHY7dNyjmbvxZEJq2D3V7a91zkR9EB7; code-fixes-component-format=LineByLine; continueCodeFindIt=2xaOBEyblGWQg2ormknJzea1vN6B4LXa4p503j7DLVERKxwq9XOdMYPJRvDb; continueCodeFixIt=O0Wmz4K5Ede4DGRP6WrJmw9kbB8XAvQoxyYLpOgzjq3QlN70VZonvM2e5wMn" -H "Sec-Fetch-Dest: empty" -H "Sec-Fetch-Mode: no-cors" -H "Sec-Fetch-Site: same-origin" -H "Content-Type: application/json" -H "Pragma: no-cache" -H "Cache-Control: no-cache" --data-raw "{""captchaId"":0,""captcha"":""-23"",""comment"":""ashgah (anonymous)"",""rating"":0}" }
```

That gives me two challenges with banners across the top of the scoreboard page.  One of the other challenges says about closing more than one banner at once.  Fortunately, I saw somewhere in the introductory sections of the "Pwning OWASP Juice Shop" [companion guide](https://pwning.owasp-juice.shop/companion-guide/latest/index.html) that you can close more than one at once by holing the `shift` button when clicking.  Turns out that wasn't a lie, and now I have another challenge solved.

### GETting On With It

At this point, I remembered that I was wanting to check whether it's possible to access feedback with a GET request.  So, I try the exact same request as before, except I change the HTTP verb from POST to GET, and in response I receive a list of all the feedback.  The only thing that's of any interest to me in there is the fact that one of the comments seems to include some HTML.  That might be how I could have a go at the "DOM XSS" challenge.  Beyond copy-pasting the provided HTML into the payload, I also try changing the username from `anonymous` to Bjoern, and the rating to -1, just to see what happens.  That pops the "Error Handling" challenge, but doesn't seem to do anything else, least of all tell me I managed the XSS.  It kind of looks like they might have some input sanitisation going on that prevents it from being totally trivial.



