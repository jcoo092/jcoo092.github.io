Title: Deploying Then Securing the OWASP Juice Shop, Part One of ? 
Lead: Describing my plan
Published: 11-07-2023
Tags:

- AppSec
- AWS
- Cloud
- Deployment
- Juice Shop
- OWASP

---

# Deploying, and then Securing, the OWASP Juice Shop Application

## Summary

I shall deploy the deliberately-vulnerable OWASP Juice Shop application to 'the cloud', and then use various techniques and tools to (attempt to) secure it.

## Introduction

[OWASP Juice Shop](https://owasp.org/www-project-juice-shop/) is one of OWASP's flagship projects, and is a deliberately-vulnerable web application.  It is used to demonstrate various vulnerabilities that can exist in real applications (including the whole of the [OWASP Top 10](https://owasp.org/www-project-top-ten/)), for the benefit of all three of builders, breakers and defenders.  The Juice Shop page on the OWASP website describes it this way:

> OWASP Juice Shop is probably the most modern and sophisticated insecure web application! It can be used in security trainings, awareness demos, CTFs and as a guinea pig for security tools! Juice Shop encompasses vulnerabilities from the entire OWASP Top Ten along with many other security flaws found in real-world applications!
> 
> Juice Shop is written in Node.js, Express and Angular. It was the first application written entirely in JavaScript listed in the OWASP VWA Directory.
>
> The application contains a vast number of hacking challenges of varying difficulty where the user is supposed to exploit the underlying vulnerabilities. The hacking progress is tracked on a score board. Finding this score board is actually one of the (easy) challenges!
>
> Apart from the hacker and awareness training use case, pentesting proxies or security scanners can use Juice Shop as a “guinea pig”-application to check how well their tools cope with JavaScript-heavy application frontends and REST APIs.

While sitting in one of many excellent talks at the recent [OWASP NZ Day 2023](https://appsec.org.nz/conference/), I had the idea of using something like Juice Shop as a ready-made web application for my own purposes.  Namely, to gain more hands-on experience actually deploying a web application into the cloud, *and* securing an existing deployed vulnerable application.  To be totally honest, the part about securing it is the thing which appeals to me more, but I figure I really should get more experience with deployment, also.

### Why Juice Shop?

Why did I pick the OWASP Juice Shop application to experiment with, you ask?  Basically, it met all the criteria I can think of.  It:

- Is written in a popular programming language that's likely to be well-supported by various tools, Node.js.
- Is already a completed application, meaning that I can get straight onto deploying it, rather than spending time finishing the functionality first.
- Is well-known and moreover has had considerable time devoted to exploring its various vulnerabilities, meaning that there are a lot of other references out there for me to use to learn about security issues that I don't spot myself. 

## Deployment

My initial plan is to try to deploy the Juice Shop straight onto AWS using a very clicky-pointy methodology, probably just as something running in a VM on an EC2 instance.  Once I have that online and can navigate to a running instance of it in my browser, I then plan to explore using various approaches to automating the deployment further.  This can include, for example, using something other than EC2 to deploy it (e.g. I believe AWS Fargate lets you deploy straight from a container without further fuss), breaking the components out into separate parts using cloud-native elements like AWS's RDS, and using Infrastructure-as-Code tools such as [Terraform](https://www.terraform.io/) or [Pulumi](https://www.pulumi.com/).

I also hope to explore using further cloud ecosystems than just AWS—especially Azure, but I will investigate others I come across which offer a suitable free tier.

## Security

The real main focus of this self-inflicted project is to explore securing a web application.  For this purpose, I roughly plan to follow the typical flow of going from planning through to development, building and deployment etc.  So, that should mean starting with exercises such as attempting threat modelling, to applying SAST tools where they offer a [free version for open source](https://owasp.org/www-community/Free_for_Open_Source_Application_Security_Tools), to applying DAST, to securing the cloud deployment.  One other thing I specifically hope to look into is both generating Software Bills of Material ([SBOMs](https://owasp.org/www-community/Component_Analysis#software-bill-of-materials-sbom))[^SaasBOM] automatically, the tooling around that (e.g. OWASP's [DependencyCheck](https://owasp.org/www-project-dependency-check/) and [DependencyTrack](https://owasp.org/www-project-dependency-track/)), and going from there to using the [SLSA framework](https://slsa.dev/).

I'm very open to suggestions of specific other tools to explore!

## Approximate Posts Plan

1. This post.  Kicking off the whole thing by stating my goals.
2. Deploying into AWS, the most basic way possible.
3. Deploying into AWS using alternative methods.
4. Deploying into AWS using IaC such as Terraform or Pulumi.
5. Threat modelling the Juice Shop.
6. Experimenting with SAST offerings.
7. Experimenting with DAST.
8. Cloud security.
9. SBOMS & SLSA.

I fully expect this plan to be revised as I go.  Moreover, as blog posts are published, I shall (try to remember to) update the plan with links to the new posts.

[^SaaSBOM]: Apparently there are also now things like a [SaaSBOM](https://owasp.org/blog/2023/06/23/CycloneDX-v1.5.html), too.