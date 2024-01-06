---
Title: Deploying Then Securing the OWASP Juice Shop, Part Six of ?
Lead: Amateurish Penetration Testing
date: 2024-01-06
draft: false
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

For reference, I ran OJS locally via Docker, using the Docker image tagged `v15.0.0`, with hash _178d89b55c46ae8b4167ddb28ce52ab4284ebce0766575c3943367ff09e27c97_.  By the time I started working on this blog post, a whole new major release had already come out, but I started the series with v15, so I'll stick with it.  I activated said Docker image using the command `docker run --rm -p 3000:3000 bkimminich/juice-shop:v15.0.0`.  I accessed the running OJS instance through my browser at `http://localhost:3000`.

If you're interested in having a go at the OJS yourself, you should definitely check out the companion guide available (as of the time of writing) at https://pwning.owasp-juice.shop/companion-guide/latest/index.html.  It also includes solutions, though I imagine there are also plenty of other blog posts and the like out there which give much better explanations of how to solve challenges than I do. 

## Wait, what happened to parts three, four and five?

When I started on this blog series, I intended to spend a few posts exploring different ways to deploy the OJS to AWS.  I did indeed do a bit of that in [post two](../two).  It turns out, however, that this was both more complicated and fiddly than I had anticipated, and moreover that it is actually really interminably dull to me.  Cloud deployments are, of course, a very important aspect of modern software development, but they're not something which excites me in the slightest.  Rather, it's the application security aspects of all this that I'm interested in.

Given all this, and that I'm currently working at a job where the cloud deployment stuff is largely all handled for me (and thus I don't _need_ to learn all about it right now), I have decided to skip trying to finish the deployment posts for the time being.  I'm certainly not ruling out going back to it, but I don't anticipate working on that any time in the near future.  Thus, this post.  That still doesn't explain the lack of part five, though.  For part five, I intended to model threats for the OJS, and then use that to help inform the later posts in this series.  After all, a _lot_ of people who know what they're talking about suggest that threat modelling is an excellent first step.  E.g. that was the key message of [the article](https://doi.org/10.1145/3608965) "Coming of Age" by Stefano Zanero, that appeared in the September 2023 edition of the Communications of the ACM.

The problem is that I have somehow managed to go years in learning about AppSec while only ever developing the most rudimentary understanding of threat modelling.  It'll take me some proper background learning to be able to produce anything worthwhile for this blog series, and I'm keen to get (re)started on it.  Thus, I'm moving straight to fumbling around poking at a local instance, to see which of the challenges I can solve based on what I currently know (and the tools I have installed on my home computer).

## First Impressions

As I said in one of the earlier posts, I managed to find the score board myself.  Honestly, there wasn't anything smart about it.  I just guessed that the URL would probably be something like `domain/scoreboard`, and tried a variation or two on that.  I probably should have taken the first piece of advice of the introductory tutorial, though, which said

> Look through the client-side JavaScript in the Sources tab for clues. Or just start URL guessing. It's up to you!

It turns out that, most of the way through the main.js file, they list all the paths for the pages on the domain.  The name of the relevant JS component is obfuscated, but, of course, they can't actually obfuscate the name of the path without breaking things.

{{< figure src="score-board_path_in_source.png" title="The path to the score board, right there in the source code." alt="A screenshot of some of the OWASP Juice Shop's source code, showing that the path to the score board is hard-coded in client-side Javascript." >}}

The score board lists all sorts of challenges, with an estimated difficulty rating for each.  For the time being, I'll focus only on challenges rated from 1-3, as higher challenges are either potentially beyond my skills and/or will take significantly more effort than I can be bothered with right now to achieve.  I'll include all the categories, though, since I don't think there's any one particular thing I'm especially good at.

Besides the challenge of finding the scoreboard itself, a handful of other challenges jump out at me as ones I might be able to achieve.  Especially some of the ones that sound like they probably are about bypassing client-side validation.  Near the bottom, however, one jumps out at me as definitely a good place to start:  "Security Policy".  It's description reads

> Behave like any "white-hat" should before getting into the action.

I'm not 100% sure what they're talking about, but it seems like a good idea to investigate this first.  The mouseover tip says something about reading the security policy, so they're _probably_ talking about making sure you understand what's considered in and out-of scope for the testing.  I'll click on the extra hints button to find out more.  That opens a link to what I think is probably the official walkthrough for the challenge.  Unfortunately, it looks like they have re-jigged the URLs on the website since v15 of OJS was released, so it gives me a missing page message.  Ah well, it's pretty easy to find that page again by just going back to the base domain and starting from there.

The write-up just has to this say

> This challenge asks you to act like an ethical hacker.  As one of the good guys, would you just start attacking an application without consent of the owner?  You also might want to read the security policy or any bug bounty program that is in place.

So yeah, they were talking about the usual practice of getting explicit permission and defining scopes, etc.  Of course, in this instance, the documentation around the OJS more or less has already given explicit permission (plus I'm only running the system locally in a Docker container on my own computer, so yes I can grant myself permission to attack that system).  I'm dimly aware of the [security.txt](https://securitytxt.org/) standard, so I'll give that a try.  My first attempt, http://localhost:3000/#/.well-known/security.txt, doesn't seem to do anything.  Maybe they don't have one?  Trying it without the `.well-known` part doesn't seem to work either.

I move on to try to search through the code again, much as with the score-board.  No luck there.  I also crack open the privacy policy (getting that solution ticked off in the process), just in case it is linked in there, but no.  At this point, I'm a bit stumped about all this.  Ordinarily, I wouldn't _nearly_ yet revert to checking the answers, but since this one is all about engaging in good practices up-front, I'll make an exception.  The [solution](https://pwning.owasp-juice.shop/companion-guide/latest/appendix/solutions.html#_behave_like_any_white_hat_should_before_getting_into_the_action) seems to say the same thing as what I tried, though.  Except, I eventually notice that in my original attempt there was a `#` in there, but there isn't in the listed solution.  Changing to http://localhost:3000/.well-known/security.txt does indeed solve the challenge.  There's possibly some relevant info in there, so I'll keep that tab open for now.

That's three down, many more to go.

## Some Basic Improper Input Validation

The below recounts the things I first tried (roughly in chronological order), but almost all of them were just uses of the same single method:  Sending a legitimate request, then using the Firefox developer tools `Edit and Resend` functionality to modify some part of a web request to achieve something that you shouldn't be able to do.  In general, this only works because the web application doesn't perform sufficient server side validation of the received inputs, instead (wrongly) trusting that the frontend validation takes care of it. 

### Zero Stars Given

Looking through the (filtered) list of challenges, some of the ones that catch my eye are from the 'Improper Input Validation' set.  Maybe because I have a little experience of doing this sort of thing in the SANS Holiday Hack Challenge, I feel like I could make a stab at it.  E.g., the challenge 'Zero Stars', which says "Give a devastating zero-star feedback to the store."  My initial thought for this is that probably there is some sort of validation on the feedback form to prevent someone submitting a zero rating, but anybody who bypasses the GUI is unstopped.  I.e., there's front-end validation, but no back-end validation.  Turns out the front-end uses a slider to set a score between 1 and 5.

{{< figure src="customer_feedback_form_empty.png" title="The customer feedback form when you first open it." alt="A screenshot of the OWASP Juice Shop's customer feedback form, as it is when you first navigate to the page." >}}

The easiest way I know of to try out fiddling with such things is to submit a valid request, and then use the browser's built-in development and debugging tools to replay the request with an updated payload.  So, F12 it is, to crack open Firefox's dev tools.  I fill out the feedback form with some dummy info and a 1-star rating, and watch the network requests.  I notice that, besides the POST request to the `/api/Feedbacks` endpoint, there are also GET requests to `whoami` and `/rest/captcha/`.  I presume the latter validates that the answer provided for the captcha challenge is correct, but I'm not sure what the other one is about.  They could well be interesting later on, but I don't _think_ I need them right now to achieve the challenge.  I also notice that I get back as part of the response the ID 8.  So there's probably seven other earlier feedback items in the database.

I do wonder at this point, though, whether I can do a GET request to the Feedbacks endpoint mentioned earlier.  I try just requesting it via cURL, but get an error an invalid token.  Presumably, I need to include some header or another for the endpoint to accept it.  I'll try again via the dev tools, but first the zero-star rating (just in case I muck something up).  I update the rating entry in the replayed request's body to 0 and hit send.  Success!  The ID on the response to this one is 9, suggesting that the feedback items are indeed stored with sequential IDs.

{{< figure src="Solved_zero_stars.png" title="One down, many to go." alt="A screenshot of the OWASP Juice Shop's challenge completion banner, stating that the 'Zero Stars' challenge was completed successfully." >}}

### While I'm Here

Looking through the list of challenges for others that I can probably do while I'm here, I notice 'CAPTCHA Bypass', which is about submitting 10 or more instances of feedback within 20 seconds.  Given that I could already replay the initial submission to do it once, if I can just repeat that same request in a for loop in a script, that should be trivial.  I also discover that Firefox very helpfully actually provides an option to put the correct cURL command to repeat the request onto the clipboard.  So, between that and a sliver of Powershell, that challenge is also already solved.  The precise command used was:

```powershell
1..10 | % { curl "http://localhost:3000/api/Feedbacks/" -X POST -H "User-Agent: Mozilla/5.0 (Windows NT 10.0; rv:121.0) Gecko/20100101 Firefox/121.0" -H "Accept: application/json, text/plain, */*" -H "Accept-Language: en-GB,en;q=0.5" -H "Accept-Encoding: gzip, deflate, br" -H "Referer: http://localhost:3000/" -H "Origin: http://localhost:3000" -H "DNT: 1" -H "Connection: keep-alive" -H "Cookie: language=en; welcomebanner_status=dismiss; continueCode=xmOK5wX8WKPpQY6Mr4neoLglBOAP9LHY7dNyjmbvxZEJq2D3V7a91zkR9EB7; code-fixes-component-format=LineByLine; continueCodeFindIt=2xaOBEyblGWQg2ormknJzea1vN6B4LXa4p503j7DLVERKxwq9XOdMYPJRvDb; continueCodeFixIt=O0Wmz4K5Ede4DGRP6WrJmw9kbB8XAvQoxyYLpOgzjq3QlN70VZonvM2e5wMn" -H "Sec-Fetch-Dest: empty" -H "Sec-Fetch-Mode: no-cors" -H "Sec-Fetch-Site: same-origin" -H "Content-Type: application/json" -H "Pragma: no-cache" -H "Cache-Control: no-cache" --data-raw "{""captchaId"":0,""captcha"":""-23"",""comment"":""ashgah (anonymous)"",""rating"":0}" }
```

That gives me two solved challenges with banners across the top of the scoreboard page.  One of the other challenges says about closing more than one banner at once.  Fortunately, I saw somewhere in the introductory sections of the "Pwning OWASP Juice Shop" [companion guide](https://pwning.owasp-juice.shop/companion-guide/latest/index.html) that you can close more than one at once by holing the `shift` button when clicking.  Turns out that wasn't a lie, and now I have another challenge solved.

### Bullying a Chatbot

There's another challenge listed "Bully Chatbot", with the description

> Receive a coupon code from the support chatbot.

It also is tagged with 'Brute Force' and 'Shenanigans'. This all leads me to suspect that if you can ask for a coupon code enough times in a sufficiently short space of time, you can get a coupon code out of it.  Which means this challenge probably needs a similar approach to the last one.  I take the exact same approach as with the 'CAPTCHA Bypass' challenge.  I open up the chatbot section, write a message reading something like "coupon", and see what happens.  The chatbot responds with something about how it can't give me a coupon code, but I now have a record of a web request asking for a coupon.  I again copy the cURL command for it out of Powershell, wrap that in the Powershell shorthand for a for loop, bump the number of iterations up to 100, and hit enter.  The confetti cannons go off, and apparently I have beat the challenge.  It looks like I managed to beat the same coupon code out of it a few times, but have to scroll back up through the output of all 100 requests to find it.

### GETting On With It

At this point, I remembered that I was wanting to check whether it's possible to access feedback with a GET request.  So, I try the exact same request as before, except I change the HTTP verb from POST to GET, and in response I receive a list of all the feedback.  The only thing that's of any interest to me in there is the fact that one of the comments seems to include some HTML.  That might be how I could have a go at the "DOM XSS" challenge.  Beyond copy-pasting the provided HTML into the payload, I also try changing the username from `anonymous` to Bjoern, and the rating to -1, just to see what happens.  That pops the "Error Handling" challenge, but doesn't seem to do anything else, least of all tell me I managed the XSS.  It kind of looks like they might have some input sanitisation going on that prevents it from being totally trivial.  I will have to come back to this.

### Repetitive Registration

By this point, I'm running out of challenges where I'm pretty sure I know from the start what to do.  One does catch my eye, though, named "Repetitive Registration".  The description of it simply says

> Follow the DRY principle while registering a user.

I'm not totally sure what this means, but I can guess that it probably is about not repeating the exact same password when registering a new user.  So, off to the new user registration section.  A random username, a random password, and a slightly different version of that password later, and it's another challenge solved.  Turns out they did indeed mean that you register a user without repeating the same password.

### Empty User Registration

Oh, turns out there's another similar challenge to the last one.  This time, register a user without even providing an email address or username.  That same old 'edit and resend' trick to the rescue!  This time, I just make the JSON for the username and password empty strings, e.g. `{ "Password": "" }`.  The confetti cannons go off, and it's another challenge done.

### "View Basket"

By now, I'm running out of challenges I think I can solve through the one trick, but I notice one last one I might just be able to achieve.  Viewing another user's basket.  I just go back to the store's front page, put something into the shopping basket, navigate to the basket view page, and find the relevant HTTP request to get the contents of the basket.  Then, I try sending that back but with a different user's ID.  Turns out the OJS apparently doesn't validate for a basket view request that the requestor is actually authorised to view that basket (usually normal customers should only be authorised to view their own basket, and denied for everybody else's).

### Moral of the Story

So by now I'm beginning to run out of ones that are totally obvious to me, and for which I don't think I need any further help.  Almost everything I did here just revolved around modifying a legitimate web request and sending it again, to make the system use its normal functionality to do something a normal user isn't meant to achieve.  The weaknesses exploited pretty much all relied on two failures in the secure development system (deliberate in the case of OJS, of course):

- A lack of server-side validation of request parameters, presumably instead relying on client-side validation.
- Assuming that people wouldn't observe actions the frontend undertakes by itself, and then imitating them in a way outside of that intended for non-malicious users.

This leads to two overarching morals of the story so far.  Namely, that you should always assume that any web request the frontend makes can and will be observed – and abused – by users, and moreover that you should _never_ rely on client-side validation to prevent invalid inputs.  Client-side validation is used to help legitimate users interact with the system sensibly, but can always be bypassed by those with ill-intent.  You _must_ use server-side validation, away from the direct control of end-users, to confirm that parameters are valid and don't enable someone to do something they shouldn't.

## OSINT

Thanks in large part to having taken part in the challenges for [CHCon 2021](https://2021.chcon.nz/), which focused pretty much entirely on OSINT, I think I might have a decent chance of completing some of the challenges under this category.  Besides the general lesson of not using secrets that are actually on the web for anyone to find, there's likely not really anything especially instructive in completing the challenges.  Thus, I'm just going to skip them in this blog post.  I am awfully curious about the one for Bender, however, given that I have seen just about every Futurama episode at least once (if not many more times than that).

## Administrivia

There are a few challenges that seem to relate to logging in as an admin user or accessing the admin section.  Presumably, if you can log in as an admin user, you can access the admin section, so it seems like focusing on that is likely to be the more useful approach.

### Admin Section

Two of the ones I can see involve logging in as the admin user in some fashion or another, but one of them just says to access the admin section of the website.  Based on that, I'm guessing that the admin section isn't actually only available to admin users.  Perhaps a link is only shown in the UI to administrative users, but anybody who guesses the correct link can find it.  The mouseover text on the hint block for the challenge seems to back that up, so I'll try that.

I first try navigating to http://localhost:3000/#/admin, but that just gives me the front page.  I then try http://localhost:3000/#/administration and find a 403 page.  Looks like there actually is some access control on this page.  I also try both without the hash, but that doesn't get me anywhere.

{{< figure src="admin_403.png" title="Access Denied" alt="A screenshot of the OWASP Juice Shop's 403 error code page banner, showing that the current user isn't permitted to view the requested web page." >}}

I realise that I'm not actually logged in as any user at this point, however.  Possibly it's open to every registered user, but blocked simply for people who aren't signed in.  So, off I go to sign in as a regular user.  Except, that also doesn't work.  I get the same result.  It seems like you do need to be an admin user to access the page.  At least I know where it is, though.

Which brings me to another point.  Any unauthenticated user should always be shown 404 error messages for any page they are not authorised for.  Otherwise, it becomes trivial to map out the real pages simply by requesting them and checking which ones return 404s vs 401s or 403s.  Whether this should also apply to authenticated but unauthorised users is much more context dependent.  If any random member of the public can create an account and thus be authenticated, you'll probably want to follow this approach.  If you only have a very small number of users, all of whom are considered known and trustworthy, it's probably OK to give them a 403 message instead.

Anyway, it looks like I will need to log in as a user with admin access to manage this one, so let's try that next.

### Admin Registration

This challenge has the description:

> Register as a user with administrator privileges.

It sounds to me like there's probably some way to specify a user's assigned role when sending the input data for creating a new user.  Perhaps the server accepts a role as input and blindly assigns that if present, rather than some server-side logic figuring out the correct role (which, for a customer-facing application will basically always be the standard user or customer role).  Actually another one to try out the Edit & Resend system with, methinks.

I'll go through the user registration process again, but this time I'll watch the request and response to create a new user.  This generates a POST request with the following request body: `{"email":"***@example.com","password":"***","passwordRepeat":"***","securityQuestion":{"id":2,"question":"Mother's maiden name?","createdAt":"2024-01-05T20:36:38.850Z","updatedAt":"2024-01-05T20:36:38.850Z"},"securityAnswer":"***"}` (bits replaced with `***` just because, even though I used garbage values).  The response body, however, has `{"status":"success","data":{"username":"","role":"customer","deluxeToken":"","lastLoginIp":"0.0.0.0","profileImage":"/assets/public/images/uploads/default.svg","isActive":true,"id":22,"email":"***@example.com","updatedAt":"2024-01-05T20:59:21.429Z","createdAt":"2024-01-05T20:59:21.429Z","deletedAt":null}}`.  I notice the role field in there.  Perhaps if I include that in my request body, too, with "admin", it might just create someone as an admin user?

I add `"role":"admin"` into the request body and resend it.  That triggers the confetti cannons for this challenge, so apparently my new user has admin privileges.  After logging in as that user, I can't see a link to any administration page in the UI, but I'll try out the link I guessed earlier.  Yes, success!  Instead of a 403 banner, I see a list of registered users, and the confetti cannons go off for the 'Admin Section' challenge.  So, y'know, remember never to accept the role specified by a user in their registration request.

I note, though, that even after accessing the page, there never seems to a link to the admin section displayed in the UI.  Possibly it was always intended that you had to access the page by knowing the link.  Which, by itself, is a terrible idea.  With it gated behind the admin role requirement, though, it's not so bad.  After all, it _should_ be very difficult to become an admin user illegitimately.

#### Cookie Monster

Looking at the cookies for the website, I see that there is one for the JWT token, which includes a listing for the user's role.  _Perhaps_ you could just change the cookie to grant yourself the admin role, and maybe the system will accept it from the cookie.  I'm not sure if that would work, though, since it looked like the cookie was actually using signatures and validation, so updating it might cause the new JWT to be rejected.  Perhaps it may be possible to change the validation algorithm used to the blank one (I think you just specify `"alg":"none"), but since the above worked, I didn't bother with trying that. 

#### Now That We're In Charge...

Since we're in the admin section and can see a list of users, let's take a squiz at what info we have on them.  Unfortunately, it seems that it's not a whole lot.  We just get an email address and the date of registration and last update.  Still, we now know the login email address for every registered user, which could well prove handy in future.

{{< figure src="user_admin_details.png" title="What administrators see about users" alt="A screenshot of the user details shown to administrators of OWASP Juice Shop, in the admin page." >}}

I also note that there is a display of the customer feedback for the OJS listed here, with trashcan icons next to each one.  One of the other challenges, 'Five-Star Feedback', has the description "Get rid of all 5-star customer feedback."  Assuming that there aren't any hidden feedback items, then it would seem like it's trivial to finish this challenge from the admin panel.  In fact, there is only one item of five-star feedback listed currently.  Hitting the trashcan icon next to that is sufficient to solve that challenge, too.

### Login Admin & Password Strength

I tried out another couple of challenges around trying to log into the administrator's account (which we know from being able to see the list of emails uses the email address `admin@juice-sh.op`), but didn't manage to get anywhere.  Thus, given that they both have associated tutorials, they seem like good candidates for the next section of this post.

## Get Help

By now I have run out of ones that I can figure out without any sort of help (and which don't sound like they'd be a whole load of work).  There are some I think I'm pretty close on, however, like the basic cross-site scripting ones or the one about finding an exposed metrics system's endpoint.  Furthermore, there are a few remaining challenges that apparently come with walkthroughs or tutorials, so I might as well work through those while I'm at it.

### XSS

The first thing I think I'll look at is revisiting the XSS challenge that I couldn't quite figure out before.  I don't want to go straight to the guided tutorial, so I'll look at the [extra hints](https://pwning.owasp-juice.shop/companion-guide/latest/part2/xss.html#_perform_a_dom_xss_attack) they provide on the website, and see if I can get far enough with just those.  The hint there basically just tells you to go look at the next hint, which is for one of the challenges that aren't actually available by default when running in Docker.  Basically, it just says to look for a part of a page where the user's input is incorporated into the page.

I went through just about every page in the whole shop, trying out the instructed HTML.  No luck with any of them.  Either they didn't have anywhere for user input, or the user input was either scrubbed appropriately or not actually displayed back.  I'm just about ready to give up and run the tutorial, when I realise that there is a website search function, and that I haven't actually tried with that yet.  I simply copy-pasted the sample command into the search box and hit enter, and the challenge is completed.  Yip, it's that simple, when you know how to do it (in this instance).

#### Bonus

Somewhere else on the scoreboard under "Bonus Payload", they list another, larger, input to try.  It's the exact same process to solve this one, too.  Except now instead of a little pop-up message, it links you to a recording the OJS jingle.  I suspect that this challenge was included more to give them a way to make people listen to the jingle, rather than for some educational benefit.

### A Modern Prometheus

The "Exposed Metrics" challenge says to find an endpoint serving up usage data for a popular monitoring system, where said system is clearly indicated to be [Prometheus](https://prometheus.io/).  I would guess that probably the endpoint in question is exposed on the default path and port, and without any auth requirements put in front of it.  Depending on what's available through it, this may or may not be a bad thing.  You should definitely default to keeping things buttoned-up, though, unless you can be _really_ sure that there's nothing sensitive in there.  You never know what a sneaky attacker [can find and exploit](https://www.microsoft.com/en-us/security/blog/2023/07/14/analysis-of-storm-0558-techniques-for-unauthorized-email-access/).

Anyway, the point is that this challenge is probably trivial if you bother to read some documentation.  In fact, most likely there will be some sort of 'getting started' document that describes the default endpoint to query.   The [page labelled](https://prometheus.io/docs/introduction/first_steps/) "First Steps" seems like a good place to look.  Yes, here we go, that page includes the sentence

> Prometheus expects metrics to be available on targets on a path of /metrics

That seems like a good first thing to try.  In this instance, it's also the last thing to try, as simply pointing one's browser to http://localhost:3000/metrics is sufficient to solve this challenge.  Turns out I didn't really need extra help on this one.  It highlights an excellent point, though, namely that you should just do the first part of the beginners' tutorial and then stop and call your production system working.  That's good for initially experimenting with and learning a new system, but it's pretty much never appropriate for actually deploying it anywhere besides your development machine.

### Return of the Administrator

For the two administrator-account-related challenges I couldn't complete above, I'll first check the hints, both in the OJS itself and in the Pwning OJS guide.

#### Login Admin

To be honest, the hints don't really give me much help with this.  I had already figured out that I probably want to have a go at the login form, but hadn't really got further than that.  One hint mentions trying to reverse the password hash, but I have no idea where to find the password hashes.  Otherwise, it's not really clear to me what they intend.  I'll have to resort to going through the tutorial.

Ah, turns out they're intending for you to use SQL injection.  To be honest, to this day I have still never really quite got the hang of SQL injection (perhaps because I'm still not all that great at writing SQL), so I probably did need the tutorial for that.  As it turns out, the magic string for this is something like `' OR true--`, which you slap into the email address.  As I understand it, the opening quote mark closes off whatever SQL query the string is put into, then the `OR true` ensures that the whole thing returns true (presumably it's assumed to be something checking that the password entered matches the password on record).  The dashes are a comment in SQLite flavoured SQL, so that anything else in the query is discarded, rather than being kept and causing syntax errors when things don't fit right.

Anyway, following the tutorial gave me some idea of what they expected, and how to do it.  Definitely worth resorting to the tutorial when all else fails.

#### Password Strength

The hints suggest that it should be fairly easy to guess the admin account's password.  I tried earlier, though, using everything I could think of like "password", "letmein", "12345", "monkey" (I hear that is a surprisingly common password), "admin", etc., and combinations of the above, with and without a trailing "!".  None of them worked.  Again, the hints suggest using the password hash, but I still have no idea where that is supposed to come from, so that won't work.  They also suggest attempting to brute force it using a password list and a script, which is a good idea, but I'm just too lazy right now.  I shall instead resort to the tutorial yet again, and see what they have to say.

Hmmm, the tutorial doesn't _really_ tell you too much, it basically just suggests to guess, then kinda lets you know when you're in the right place.  Anyway, it turns out the admin password is simply `admin123`.  I suppose I must have tried `admin12345`, but never though to try it out without the last two digits.  The lessons here being things like never using such an obvious and easily guessable password,[^usepasswordmanager] that manually trying out some guesses can get you a long way, and that manually trying out guesses can be error-prone and scripting something to brute force it might well be easier in the long run.  Assuming that people are foolish enough to use a password that can easily be gathered from such a list.

[^usepasswordmanager]:  Ideally, you would use a password manager to create and store long, random passwords for you.  Or if you need to be able to memorise it, get the password manager to generate a good, _actually random_ passphrase for you (all good managers come with such a generator these days).

### Help Unwanted

Ok, turns out that I didn't _really_ need help for some of those challenges.  If I had kept going with others (e.g. those with more than two stars) I almost certainly would have required it, though.  At this point, however, I'm getting bored with writing this blog post (not with attempting to hack the OJS, though), so I'm gonna call it quits here.

## Summary

So there you have it.  A reasonable number of challenges solved, mostly just by tampering with the body of legitimate GET or POST requests and resending them.  In every one of those cases, the OJS is too trusting or accepting of user-supplied inputs, and performs insufficient server-side validation.  Perhaps in some cases it inappropriately relies on client-side validation to stop people submitting iffy values, which is a definite mistake.  Meanwhile, other challenges can be solved by a little (vaguely-educated) guesswork, or reading a little documentation.  The main morals of this story might be:

- Never trust user inputs.
- Always perform server-side validation of user inputs, rather than relying on your frontend UI to stop miscreants from submitting invalid stuff.
- Don't leave anything sensitive exposed without further authorisation requirements, especially if that is available in a default or easily guessable location.
- Be cautious about what can happen if users automate making requests.  Strongly consider putting rate limiting on any API endpoint where a legitimate user is unlikely to need to use it at extremely high rates.

Of course, there are still myriad more challenges in the OJS to complete, many of which will likely require other techniques to solve, and other preventative measures to defend against.  Thus, the list above is almost certainly thoroughly incomplete.  Cybersecurity:  It's a deep topic :)

If you think this post seemed interesting, I fully encourage you to have a go yourself.  Give it your best shot without using any hints, but once you have spent a decent amount of time on something and haven't gotten any further, there's nothing wrong with looking for some help.  After all, if you're giving it a go, you're probably not claiming to be an expert already, and sometimes the only way to learn is to get some help from elsewhere.  Definitely some of the techniques I have seen used are things that I would likely never come up with myself, but once I have experience with them, perhaps by working through a walkthrough myself, I might know to try them out.

If you'd like to learn more about hacking websites/web applications, the [Portswigger Web Security Academy](https://portswigger.net/web-security) is generally extremely well-reputed for a free resource.  I believe it's somewhat focused on Portswigger's own product, Burp Suite, but that's just a (widely-used by professionals) tool to make doing a lot of these things easier, rather than something magical which enables people to do something they couldn't otherwise achieve.  You can also have a go at the most recent few year's [SANS Holiday Hack Challenges](https://www.sans.org/holidayhack).

For paid training resources, I understand that [Pentester Academy](https://www.pentesteracademy.com/) is pretty well-regarded, too.  There are also things like [Hack The Box](https://www.hackthebox.com/) and [Try Hack Me](https://tryhackme.com/).  [OWASP](https://owasp.org/) members receive complementary membership to an OWASP instance of [SecureFlag](https://www.secureflag.com/owasp).  I don't know exactly how well-regarded each of them is, but I don't think any of them have particularly bad reputations.  There are undoubtedly a vast panoply more resources, both free and paid, out there for web application security, but some of the above should be a good place to get started I imagine.

Lastly, if you think this stuff seems pretty neat, and you might like to do it professionally, you could look at becoming a penetration tester.  It's not the right path for me, but it might be for you.  It generally pays pretty well, and if they get bored with pentesting, pentesters generally seem to be able to go on to high-flying jobs in the cyber defence side of things.  Having not done it myself, I can't speak too much to how to go about trying to get in, so I'll point you to Simon Howard's excellent resource on the topic:  [Getting Started as a Penetration Tester in NZ (2023 Edition)](https://www.linkedin.com/pulse/getting-started-penetration-tester-nz-2023-edition-simon-howard).  Mr Howard is very well respected in the New Zealand security industry, and can be considered reasonably authoritative on the matter.  The post is New Zealand-focused, but I imagine a huge amount of the information applies in most countries around the world.  By the time you read this, he may well have written a later edition, so it might be worth seeing if you can find that one.

Oh, and last of all but most importantly:  **DO NOT COMMIT CRIMES**.  Use your newfound hacking powers for good, and _always_ get permission (preferably explicit written permission) from the owners & administrators of any system you target, _before_ you take any action against it.  Seriously, the difference between criminal acts and a paying job can sometimes be as simple as whether you asked first.  If people say no, then move on.  There are plenty of targets out there already for you to practice with.