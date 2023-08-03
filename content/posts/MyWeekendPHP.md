---
Title: My Weekend With PHP
Lead: My first impressions of the language
date: 2023-07-30
draft: false
Tags:

- Exercism
- First Impressions
- PHP
- Programming Languages
---

# My Weekend With PHP

For certain reasons, I spent most of this weekend ~~diving into~~ scratching the surface of [PHP](https://www.php.net/), a language of which I have been aware for a _long_ time but never actually touched until now.  In case you somehow are reading this yet have never heard of PHP, it summarises itself thusly:

> A popular general-purpose scripting language that is especially suited to web development.
> Fast, flexible and pragmatic, PHP powers everything from your blog to the most popular websites in the world.

For reference, I was using PHP 8.2, specifically.  That being the most recent official release available at the time (I'm pretty sure 8.3 is already in pre-release, but I didn't need or really want to dive into the cutting edge).  I have the impression that there probably have been a number of changes from 8.0 onwards that have made it a much better language to my tastes, but I can't say for sure.

I spent most of my time on [Exercism's](https://exercism.org/) [PHP](https://exercism.org/tracks/php) track, working through the concepts curriculum there.

In the end, my impression of the language is mixed.  Which, considering I had rather low expectations going in, is actually a net win for PHP.

## Preconceptions

To be honest, I didn't have a great image of PHP in my head before this.  To me, it mostly meant giant messes of spaghetti code that started off as something that somebody cobbled together in a few hours, and which had grown out of control since.  Or, even worse, the thing that phishing crews used to operate their kits without giving all their secrets away in client-side Javascript.  And generally, those phishing kits are Frankenstein's monsters of garbage code pilfered from everywhere and anywhere, then beaten into shape just enough to do what they want (i.e., steal people's personal information).  It also reminds me, negatively, of websites from the 1990s.  Oh, and complaints from my erstwhile colleagues at Cosive about [MISP](https://www.misp-project.org/) when working on [CloudMISP](https://www.cosive.com/cloud-misp).[^cloudmisp]

[^cloudmisp]:  Incidentally, if you happen to be looking into setting up a MISP instance for your organisation, I _strongly_ recommend looking into CloudMISP.  I was never involved in the work on CloudMISP, but I had enough experience with it while developing a plugin for a client's product to act as a bridge between MISP and that product to know it can be a pain in the neck.  The amount of hassle using the off-the-shelf SaaS product can save you is, well, it wouldn't take long before the price is outweighed by the hair not torn out of your head.

What little I did know of it was that (at least, so I thought) it is a dynamically-typed language, with a pretty basic type system.[^golang]  Furthermore, it is interpreted rather than compiled.  Those aspects combined are just about some of my least favourite for a programming language, so I wasn't expecting great things.  And, I had the impression it was pretty heavily imperative and/or object-oriented, which aren't inherently bad, but those who have spent more than five seconds talking to me about programming languages know that I favour the functional-esque ones.  Or the more declarative ones, at least.  So yeah, I wasn't expecting great things.

[^golang]:  At least the dynamic typing does a lot to alleviate the need for generics, making the basic type system much less underwhelming than Golang's until v1.19...

## Running PHP and PHPUnit on Windows 11

Using PHP on Windows 11 is not trivial, but it's also not too hard if you're willing to work with WSL or some form of virtualization such as Docker or Vagrant/virtual machines.  There are official PHP Docker images which let you run PHP scripts pretty easily.  Sadly, as soon as you want to use [PHPUnit](https://phpunit.de/), which I understand to be the dominant unit testing framework in PHP these days, things get much more difficult.

PHPUnit is not distributed in the official PHP Docker image.  There are two main ways to install it into a given project.  Using PHP's [Composer dependency manager](https://getcomposer.org/) (think _roughly_ .NET's NuGet) or by using a [PHAR](https://www.php.net/manual/en/book.phar.php) (think loosely the same as a Java JAR).  Composer seems to be the generally-recommended option, but it turns out that Composer itself isn't included in the official PHP Docker image.  In one way, this makes sense since it's a system developed by people outside the PHP developer team itself, but in another way it seems a bit silly since it appears to be the _de facto_ standard these days for managing dependencies.  Moreover, it turns out that installing Composer seems to be inevitably a long and convoluted process.

Sadly, the problems didn't stop once I got Composer installed in a Docker container.  In theory, all that is required at that point is to use `composer require phpunit/phpunit`, and away Composer will go and install it for you.  Which it did, but for some reason no matter what I tried, I could never get the darn thing to run successfully on the tests provided in the Exercism exercises.  I eventually gave up on trying to do it myself at the command line, and turned to IntelliJ IDEA, since I figured with the PHP plugin it'd make configuring things much easier.  That proved to be a vain hope, however.  After spending probably about two more hours fiddling about trying to figure out why I couldn't get things working, I gave up again and installed the PHP-specific PHPStorm IDE (which, in theory, is just a stripped down version of IntelliJ).

Even then, the difficulties weren't over.  Long story, short, I eventually figured out (with some assistance from the PHPStorm documentation online, which is actually somewhat decent) how to get it configured to use the PHP installation inside a Docker container, and then configure it to use PHPUnit.  The short answer is that I had to download the PHPUnit PHAR, stick it into the given PHP project's top-level directory, and then point PHPStorm at it.  Which I had to do every time I went to a new exercise.  In JetBrains' defence, they probably weren't designing their product for people who keep switching to a new project every hour or so.  Finally, though, I could actually run the supplied unit tests.

On the point of PHP Docker images, JetBrains themselves publish a Dockerfile for PHP development.  At first, I was hesitant to use it, since it hadn't been updated in six months.  I eventually realised, however, that it would base itself on the latest official PHP 8.2 at the time the container is built, so in fact it should be completely up-to-date for most purposes.  It just adds on a few extra bits atop the official image.  It actually worked pretty well once I had things figured out.  Though, I can't say that PHPStorm felt as polished as say, Rider.  Perhaps it was simply that I didn't get deep enough into a big PHP project to experience it, however.

## My Impressions

As mentioned earlier, my impression of the language is mixed.  It exceeded my very low initial expectations, but I wasn't blown away by it either.

The language just feels a bit messy to me.  I think I saw a quote somewhere from the original PHP creator that he never had any sort of plan or roadmap for it, he just kept chucking in the next feature that he thought would make sense.  To be honest, that does show through a bit.  For one thing, there are an enormous number of random top-level functions in the global scope (rather than behind certain namespaces).  I have the impression, though, that someone has been slowly rationalising the language and tidying it up over the past few minor releases or so, based on notes I saw scattered throughout the PHP documentation.  I personally think this actually bodes well for the future of the language.

Another thing I didn't love was the limited range of built-in types.  Probably the utterly gigantic BCL in .NET has spoiled me, but PHP has a handful of primitive types, associative arrays, and...  that's about it (from what I saw, at least, though I know that Laravel has some nice extras at least).  The design of the associative array is fairly reasonable on the whole, however.  I'm less-than-thrilled about the fact that you can only use integers and strings as keys, but the design with integers is quite intelligent.  It means that you can use an array basically like a C-style array without ever noticing that it's actually associative if you prefer.  I also didn't really love that there isn't a separate char type to strings (you can reduce a string to individual characters, but those characters still have string type I believe).

On the positive side, there are some really nice utility functions in the global namespace.  For instance, working dates and times is quite easy with the in-built facilities.  The gigasecond Exercsim exercise reduces to two simple lines with PHP's functionality.  Many of the others weren't too far off that, either.  PHP also seemingly includes reasonably solid UTF-8 support on a like-for-like basis for many string handling functions—mostly, you just need to prepend the string function's name with `mb_` (for multibyte).

I was also pleasantly surprised at how much support there was for typical functional concepts like higher-order functions and the usual `map`/`filter`/`reduce` style suspects.  Using them can be a bit clunky, but they're there, and they do what you want.  This was definitely one thing I wasn't expecting.

## Conclusion

I don't see me rushing out to try to use PHP for just about anything, nor immediately recommending it to others.  At the same time, I also don't see me trying to avoid it like the plague.  Almost certainly I wouldn't choose PHP for any new project, and nor would I actively seek out jobs where working with PHP is a big part of it.  Yet, equally, if I was told that there would be some PHP work in a job, that wouldn't put me off.

I'm sure there's more I could (perhaps should) say, but I can't think of it right now.  I'll come back and update this post in the future if I think of anything else.