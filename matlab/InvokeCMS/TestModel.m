function [ modelDescription ] = TestModel()
%TESTMODEL Summary of this function goes here
%   Detailed explanation goes here

    header = ['(import (emodl cmslib))' '(start-model "matlab")'];
    species = '(species S 990) (species E) (species I 10) (species R)';
    params = '(param Ki 0.0005) (param Kl 0.2) (param Kr 0.143) (param Kw 0.0074)';
    observables = '(observe susceptible S) (observe exposed E) (observe infected I) (observe recovered R)';
    reactions = '(reaction exposure (S) (E) (* S I Ki)) (reaction infection (E) (I) (* E Kl)) (reaction recovery (I) (R) (* I Kr)) (reaction waning (R) (S) (* R Kw))';
    footer = '(end-model)';
    modelDescription = strcat(header, species, params, observables, reactions, footer);

end

