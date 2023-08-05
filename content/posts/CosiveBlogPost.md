---
Title: Securing REST APIs Against Data Leaks
Lead: Cross-referencing a blog written while at my former employer
date: 2023-08-05
draft: false
Tags:

- AppSec
- APIs
- Data Leaks
- REST
---

# Securing REST API Endpoints (or 15 Steps to Avoid Another Optus)

While I was working at [Cosive](https://www.cosive.com/), I wrote [a blog post](https://www.cosive.com/blog/2022/10/11/securing-rest-api-endpoints-or-15-steps-to-avoid-another-optus) outlining some of the usual advice around securing REST API endpoints, with a particular view to preventing data leaks.  This was inspired by the then-recent leak of customer personally identifiable information from the systems of Australian telecommunications outfit Optus.  It was titled "Securing REST API Endpoints (or 15 Steps to Avoid Another Optus)".  The management at Cosive have agreed that I can put up a blog post of my own linking to the blog post on their website, but with any updates I may wish to make to it posted here.  So, that's what this blog post is.

In case you missed the link earlier, here it is again:  https://www.cosive.com/blog/2022/10/11/securing-rest-api-endpoints-or-15-steps-to-avoid-another-optus

## Subsequent Developments

Shortly after the original blog post was, er, posted, myself and the great Tash Postolovski recorded a discussion of the material in the post as [a podcast episode](https://www.cosive.com/podcast/2022/10/27/episode-003-securing-rest-api-endpoints-or-how-to-avoid-another-optus-with-james-cooper) for the [Cosive podcast](https://www.cosive.com/podcast).

I subsequently used the blog post as the basis for a presentation on the same topic, which has ended up with the revised title "Securing REST APIs Against Data Leaks:  Or, How to Avoid Another 'Optus'."  The tweak to the title was intended to convey more clearly the fact that the presentation is focused specifically on technical and cultural/organisational measures to take to prevent leaking sensitive data, rather than any other area of security.  The presentation has now been given to [OWASP NZ Day 2023](https://appsec.org.nz/conference/speakers.html#cooper-securing-api-endpoints), and ISIG Auckland.  I am expecting to present in to ISIG Hamilton in a little while.

## Additions, Revisions and Clarifications

If I think of anything else I want to say, explain or change my mind about, I'll describe it here.

### Don't Keep Data

Something I completely forgot to mention in the original blog post (and the first run of the presentation) is arguably the most important thing to say.  Namely, if you don't _need_ to keep some data, then don't do it!  In fact, better never to collect them in the first place.  If you don't want to leak sensitive data, then never getting your hands on them is _by far_ the best way to keep them safe.  Bruce Schneier, [when discussing the same idea](https://www.schneier.com/essays/archives/2016/03/data_is_a_toxic_asse.html), calls data "toxic".

In the case of Optus, it might well be that they were required by law or legitimate business purpose to collect and retain those data.  I simply don't know.  I would strongly question, however, just how many organisations truly need to all the screeds of data they collect, compared to how much they either simply sell to data brokers or think they will somehow turn into future profits because "data is the new oil".

So yeah, don't do it if you can possibly avoid it.  I would recommend that organisations take the approach of at least trying to ensure that they are compliant with the EU's GDPR.  It covers some of the same ground as this point, and could well help to cover one's rear end down the line.[^avoidedunmeasured]  Especially since the GDPR can suddenly rear its head for all sorts of organisations who think they have nothing to do with it.  Even better if you can go even further.  Or, y'know, never collect any data, and then you don't have to worry.

[^avoidedunmeasured]:  Sadly, the avoided costs from data breaches that never happen are pretty much impossible to measure and quantify.  Thus, most businesses have a tendency to perceive security measures as a cost, rather than an overall saving.  This is especially true for something like a data breach, where the prevention of one is just seen as things working as they should (which is true), leading to people further discounting the benefit.